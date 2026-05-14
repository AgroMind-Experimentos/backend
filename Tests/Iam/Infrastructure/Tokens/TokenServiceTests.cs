using EcotrackPlatform.API.Iam.Infrastructure.Tokens;
using EcotrackPlatform.API.Profiles.Domain.Model.Aggregates;
using EcotrackPlatform.API.Profiles.Domain.Model.ValueObjects;
using Microsoft.Extensions.Configuration;

namespace EcotrackPlatform.Tests.Iam.Infrastructure.Tokens;

[TestFixture]
public class TokenServiceTests
{
    private Mock<IConfiguration> _configurationMock;

    [SetUp]
    public void Setup()
    {
        _configurationMock = new Mock<IConfiguration>();
    }

    [Test]
    public void GenerateToken_ValidProfile_ShouldReturnJwtString()
    {
        var jwtKey = "super_secret_key_that_is_long_enough_for_hmac256_1234567890!";
        
        var configSectionMock = new Mock<IConfigurationSection>();
        configSectionMock.Setup(a => a.Value).Returns(jwtKey);
        
        _configurationMock.Setup(c => c["Jwt:Key"]).Returns(jwtKey);
        _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("Ecotrack");
        _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("EcotrackUsers");

        var expirationSectionMock = new Mock<IConfigurationSection>();
        expirationSectionMock.Setup(x => x.Value).Returns("8");
        _configurationMock.Setup(c => c.GetSection("Jwt:ExpirationHours")).Returns(expirationSectionMock.Object);

        var service = new TokenService(_configurationMock.Object);
        var profile = new Profile("test@example.com", "Test User", "hash", UserRole.Agronomist);

        var token = service.GenerateToken(profile);

        Assert.That(token, Is.Not.Null.And.Not.Empty);
        
        var parts = token.Split('.');
        Assert.That(parts.Length, Is.EqualTo(3), "El token generado no tiene formato JWT válido.");
    }
}
