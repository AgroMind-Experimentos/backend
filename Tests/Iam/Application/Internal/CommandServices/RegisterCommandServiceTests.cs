using EcotrackPlatform.API.Iam.Application.Internal.CommandServices;
using EcotrackPlatform.API.Profiles.Domain.Model.Aggregates;
using EcotrackPlatform.API.Profiles.Domain.Model.ValueObjects;
using EcotrackPlatform.API.Profiles.Domain.Repositories;
using EcotrackPlatform.API.Shared.Domain.Repositories;

namespace EcotrackPlatform.Tests.Iam.Application.Internal.CommandServices;

[TestFixture]
public class RegisterCommandServiceTests
{
    private Mock<IProfileRepository> _profileRepositoryMock;
    private Mock<IUnitOfWork> _uowMock;

    [SetUp]
    public void Setup()
    {
        _profileRepositoryMock = new Mock<IProfileRepository>();
        _uowMock = new Mock<IUnitOfWork>();
    }

    [Test]
    public async Task RegisterAsync_ValidData_ShouldReturnSuccessAndCreateProfile()
    {
        var service = new RegisterCommandService(_profileRepositoryMock.Object, _uowMock.Object);
        var email = "newuser@test.com";
        var password = "SecurePassword123!";
        var displayName = "New User";
        var role = UserRole.Farmer;

        _profileRepositoryMock.Setup(repo => repo.FindByEmailAsync(email))
            .ReturnsAsync((Profile?)null);

        var result = await service.RegisterAsync(email, password, displayName, role);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Profile, Is.Not.Null);
        Assert.That(result.Profile!.Email, Is.EqualTo(email));
        Assert.That(result.Profile.DisplayName, Is.EqualTo(displayName));
        
        _profileRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Profile>()), Times.Once);
        _uowMock.Verify(uow => uow.CompleteAsync(), Times.Once);
    }

    [Test]
    public async Task RegisterAsync_EmailAlreadyExists_ShouldReturnError()
    {
        var service = new RegisterCommandService(_profileRepositoryMock.Object, _uowMock.Object);
        var email = "existing@test.com";
        var existingProfile = new Profile(email, "Existing", "hash", UserRole.Farmer);

        _profileRepositoryMock.Setup(repo => repo.FindByEmailAsync(email))
            .ReturnsAsync(existingProfile);

        var result = await service.RegisterAsync(email, "Password123!", "New User", UserRole.Farmer);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.EqualTo(RegisterError.EmailAlreadyExists));
        _profileRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Profile>()), Times.Never);
        _uowMock.Verify(uow => uow.CompleteAsync(), Times.Never);
    }

    [TestCase("", "password", "name")]
    [TestCase("email@test.com", "", "name")]
    [TestCase("email@test.com", "password", "")]
    public async Task RegisterAsync_InvalidInput_ShouldReturnError(string email, string password, string displayName)
    {
        var service = new RegisterCommandService(_profileRepositoryMock.Object, _uowMock.Object);
        var result = await service.RegisterAsync(email, password, displayName, UserRole.Farmer);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.EqualTo(RegisterError.InvalidInput));
    }
}
