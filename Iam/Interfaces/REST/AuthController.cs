using Microsoft.AspNetCore.Mvc;
using EcotrackPlatform.API.Iam.Application.Internal.CommandServices;
using EcotrackPlatform.API.Iam.Interfaces.REST.Resources;

namespace EcotrackPlatform.API.Iam.Interfaces.REST;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AuthCommandService _auth;

    public AuthController(AuthCommandService auth) => _auth = auth;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterResource body)
    {
        var profile = await _auth.RegisterAsync(body.Email, body.Password, body.DisplayName, body.Role);
        if (profile is null)
            return BadRequest(new { message = "Email already in use or invalid data. Password must be at least 6 characters." });

        return Ok(new
        {
            message = "Registration successful. You can now log in.",
            userId = profile.Id,
            email = profile.Email,
            displayName = profile.DisplayName
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginResource body)
    {
        var ua = Request.Headers.UserAgent.ToString();
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        var result = await _auth.LoginAsync(body.Email, body.Password, ua, ip);
        if (result is null) return Unauthorized(new { message = "Invalid credentials" });

        return Ok(new
        {
            token = result.Token,
            expiresAt = result.Session.ExpiresAt,
            userId = result.Session.ProfileId
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        if (!Request.Cookies.TryGetValue("sid", out var sid) || !Guid.TryParse(sid, out var id))
        {
            Response.Cookies.Delete("sid");
            return Ok(new { message = "Logged out" });
        }

        await _auth.LogoutAsync(id);
        Response.Cookies.Delete("sid");
        return Ok(new { message = "Logged out" });
    }
}
