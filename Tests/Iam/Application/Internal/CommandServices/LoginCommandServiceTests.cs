using EcotrackPlatform.API.Iam.Application.Internal.CommandServices;
using EcotrackPlatform.API.Iam.Domain.Model.Aggregates;
using EcotrackPlatform.API.Iam.Domain.Repositories;
using EcotrackPlatform.API.Iam.Domain.Services;
using EcotrackPlatform.API.Profiles.Domain.Model.Aggregates;
using EcotrackPlatform.API.Profiles.Domain.Model.ValueObjects;
using EcotrackPlatform.API.Profiles.Domain.Repositories;
using EcotrackPlatform.API.Shared.Domain.Repositories;
using Microsoft.AspNetCore.Identity;

namespace EcotrackPlatform.Tests.Iam.Application.Internal.CommandServices;

[TestFixture]
public class LoginCommandServiceTests
{
    private Mock<IProfileRepository> _profileRepositoryMock;
    private Mock<IAuthSessionRepository> _sessionRepositoryMock;
    private Mock<IUnitOfWork> _uowMock;
    private Mock<ITokenService> _tokenServiceMock;

    [SetUp]
    public void Setup()
    {
        _profileRepositoryMock = new Mock<IProfileRepository>();
        _sessionRepositoryMock = new Mock<IAuthSessionRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _tokenServiceMock = new Mock<ITokenService>();
    }

    [Test]
    public async Task LoginAsync_ValidCredentials_ShouldReturnTokenAndSession()
    {
        var service = new LoginCommandService(
            _profileRepositoryMock.Object, 
            _sessionRepositoryMock.Object, 
            _uowMock.Object, 
            _tokenServiceMock.Object);

        var email = "user@test.com";
        var password = "ValidPassword123!";
        var user = new Profile(email, "User", "tempHash", UserRole.Agronomist);
        
        var hasher = new PasswordHasher<Profile>();
        user.SetPasswordHash(hasher.HashPassword(user, password));

        var expectedToken = "jwt_fake_token";

        _profileRepositoryMock.Setup(repo => repo.FindByEmailAsync(email)).ReturnsAsync(user);
        _tokenServiceMock.Setup(ts => ts.GenerateToken(user)).Returns(expectedToken);

        var result = await service.LoginAsync(email, password, "Mozilla", "127.0.0.1");

        Assert.That(result.Success, Is.True);
        Assert.That(result.Token, Is.EqualTo(expectedToken));
        Assert.That(result.Session, Is.Not.Null);
        Assert.That(result.User, Is.EqualTo(user));

        _sessionRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<AuthSession>()), Times.Once);
        _uowMock.Verify(uow => uow.CompleteAsync(), Times.Once);
    }

    [Test]
    public async Task LoginAsync_UserNotFound_ShouldReturnInvalidCredentials()
    {
        var service = new LoginCommandService(
            _profileRepositoryMock.Object, 
            _sessionRepositoryMock.Object, 
            _uowMock.Object, 
            _tokenServiceMock.Object);

        _profileRepositoryMock.Setup(repo => repo.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((Profile?)null);

        var result = await service.LoginAsync("wrong@test.com", "pass", null, null);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.EqualTo(LoginError.InvalidCredentials));
    }

    [Test]
    public async Task LoginAsync_WrongPassword_ShouldReturnInvalidCredentials()
    {
        var service = new LoginCommandService(
            _profileRepositoryMock.Object, 
            _sessionRepositoryMock.Object, 
            _uowMock.Object, 
            _tokenServiceMock.Object);

        var email = "user@test.com";
        var user = new Profile(email, "User", "hash", UserRole.Agronomist);
        var hasher = new PasswordHasher<Profile>();
        user.SetPasswordHash(hasher.HashPassword(user, "CorrectPassword123!"));

        _profileRepositoryMock.Setup(repo => repo.FindByEmailAsync(email)).ReturnsAsync(user);

        var result = await service.LoginAsync(email, "WrongPassword!!!", null, null);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.EqualTo(LoginError.InvalidCredentials));
    }
}
