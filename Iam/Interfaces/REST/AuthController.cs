using EcotrackPlatform.API.Iam.Application.Internal.CommandServices;
using Microsoft.AspNetCore.Mvc;
using EcotrackPlatform.API.Iam.Interfaces.REST.Resources;

namespace EcotrackPlatform.API.Iam.Interfaces.REST;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(
    RegisterCommandService registerService,
    LoginCommandService loginService,
    LogoutCommandService logoutService)
    : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterResource body)
    {
        var result = await registerService.RegisterAsync(body.Email, body.Password, body.DisplayName, body.Role);

        if (result.Success)
        {
            return Ok(new
            {
                message = "Registration successful.",
                userId = result.Profile!.Id,
                email = result.Profile.Email,
                displayName = result.Profile.DisplayName
            });
        }

        return result.Error switch
        {
            RegisterError.EmailAlreadyExists => Conflict(new { message = "Email is already in use." }),
            RegisterError.InsecurePassword => BadRequest(new { message = "Password must contain at least 8 characters, an uppercase letter, a lowercase letter and a number." }),
            RegisterError.InvalidInput => BadRequest(new { message = "Invalid data provided." }),
            _ => StatusCode(500)
        };
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginResource body)
    {
        var ua = Request.Headers.UserAgent.ToString();
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        var result = await loginService.LoginAsync(body.Email, body.Password, ua, ip);

        if (result.Success)
        {
            return Ok(new
            {
                token = result.Token,
                expiresAt = result.Session!.ExpiresAt,
                userId = result.Session.ProfileId,
                role = result.User!.Role.ToString(),
                displayName = result.User.DisplayName
            });
        }

        return result.Error switch
        {
            LoginError.InvalidCredentials => Unauthorized(new { message = "Invalid email or password." }),
            _ => StatusCode(500)
        };
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        if (!Request.Cookies.TryGetValue("sid", out var sid) || !Guid.TryParse(sid, out var id))
        {
            Response.Cookies.Delete("sid");
            return Ok(new { message = "Logged out" });
        }

        var result = await logoutService.LogoutAsync(id);
        Response.Cookies.Delete("sid");

        if (result.Success) return Ok(new { message = "Logged out" });

        return result.Error switch
        {
            LogoutError.SessionNotFoundOrInactive => NotFound(new { message = "Session is already invalid or does not exist." }),
            _ => StatusCode(500)
        };
    }
}