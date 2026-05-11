using EcotrackPlatform.API.Iam.Application.Internal.CommandServices;
using EcotrackPlatform.API.Iam.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using EcotrackPlatform.API.Iam.Interfaces.REST.Resources;
using Swashbuckle.AspNetCore.Annotations;

namespace EcotrackPlatform.API.Iam.Interfaces.REST;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(
    RegisterCommandService registerService,
    LoginCommandService loginService,
    LogoutCommandService logoutService,
    ChangePasswordCommandService changePasswordService,
    IAuthSessionRepository sessions)
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
                message = "registerSuccess",
                userId = result.Profile!.Id,
                email = result.Profile.Email,
                displayName = result.Profile.DisplayName
            });
        }

        return result.Error switch
        {
            RegisterError.EmailAlreadyExists => Conflict(new { message = "emailAlreadyInUse" }),
            RegisterError.InsecurePassword => BadRequest(new { message = "insecurePassword" }),
            RegisterError.InvalidInput => BadRequest(new { message = "badRequest" }),
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
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Expires = result.Session!.ExpiresAt
            };

            Response.Cookies.Append("token", result.Token!, cookieOptions);
            Response.Cookies.Append("sid", result.Session.Id.ToString(), cookieOptions);

            return Ok(new
            {
                expiresAt = result.Session.ExpiresAt,
                userId = result.Session.ProfileId,
                role = result.User!.Role.ToString(),
                displayName = result.User.DisplayName
            });
        }

        return result.Error switch
        {
            LoginError.InvalidCredentials => Unauthorized(new { message = "invalidCredentials" }),
            _ => StatusCode(500)
        };
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        if (!Request.Cookies.TryGetValue("sid", out var sid) || !Guid.TryParse(sid, out var id))
        {
            Response.Cookies.Delete("sid");
            return Ok(new { message = "logoutSuccess" });
        }

        var result = await logoutService.LogoutAsync(id);
        Response.Cookies.Delete("sid");
        Response.Cookies.Delete("token");

        if (result.Success) return Ok(new { message = "logoutSuccess" });

        return result.Error switch
        {
            LogoutError.SessionNotFoundOrInactive => NotFound(new { message = "activeSessionNotFound" }),
            _ => StatusCode(500)
        };
    }

    [HttpPost("password")]
    [SwaggerOperation(Summary = "Cambiar contraseña del usuario actual")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordResource resource)
    {
        var pid = await GetCurrentProfileIdAsync();
        if (pid is null) return Unauthorized();

        var result = await changePasswordService.ChangePasswordAsync((int)pid, resource.CurrentPassword, resource.NewPassword);

        if (result.Success)
        {
            return Ok(new { message = "updateSuccess" });
        }

        return result.Error switch
        {
            ChangePasswordError.InvalidInput => BadRequest(new { message = "badRequest" }),
            ChangePasswordError.InsecurePassword => BadRequest(new { message = "insecurePassword" }),
            ChangePasswordError.InvalidCurrentPassword => BadRequest(new { message = "invalidCurrentPassword" }),
            ChangePasswordError.ProfileNotFound => NotFound(new { message = "profileNotFound" }),
            _ => StatusCode(500)
        };
    }

    private async Task<int?> GetCurrentProfileIdAsync()
    {
        if (!Request.Cookies.TryGetValue("sid", out var sid)) return null;
        if (!Guid.TryParse(sid, out var sidGuid)) return null;
        var s = await sessions.FindByIdAsync(sidGuid);
        return (s is not null && s.IsActive()) ? s.ProfileId : null;
    }
}