using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EcotrackPlatform.API.Iam.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using EcotrackPlatform.API.Profiles.Domain.Model.Aggregates;

namespace EcotrackPlatform.API.Iam.Infrastructure.Tokens;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config) => _config = config;

    public string GenerateToken(Profile profile)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddHours(_config.GetValue<int>("Jwt:ExpirationHours", 8));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, profile.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, profile.Email),
            new Claim("name", profile.DisplayName),
            new Claim("role", profile.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
