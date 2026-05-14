using EcotrackPlatform.API.Iam.Domain.Model.Commands;
using EcotrackPlatform.API.Profiles.Application.Internal.CommandServices;
using EcotrackPlatform.API.Profiles.Domain.Model.Aggregates;
using EcotrackPlatform.API.Profiles.Domain.Model.Commands;
using EcotrackPlatform.API.Profiles.Domain.Model.ValueObjects;
using EcotrackPlatform.API.Profiles.Domain.Repositories;
using EcotrackPlatform.API.Shared.Domain.Repositories;
using Microsoft.AspNetCore.Identity;

namespace EcotrackPlatform.Tests.Profiles.Application.Internal.CommandServices;

[TestFixture]
public class ProfileCommandServiceTests
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
    public async Task CreateAsync_ValidCommand_ShouldCreateAndReturnProfile()
    {
        var service = new ProfileCommandService(_profileRepositoryMock.Object, _uowMock.Object);
        var command = new CreateProfileCommand("test@email.com", "Password123!", "Test User", UserRole.Farmer);

        _profileRepositoryMock.Setup(repo => repo.FindByEmailAsync(command.Email))
            .ReturnsAsync((Profile?)null);

        var result = await service.CreateAsync(command);

        result.Should().NotBeNull();
        result.Email.Should().Be("test@email.com");
        
        _profileRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Profile>()), Times.Once);
        _uowMock.Verify(uow => uow.CompleteAsync(), Times.Once);
    }

    [Test]
    public void CreateAsync_ExistingEmail_ShouldThrowInvalidOperationException()
    {
        var service = new ProfileCommandService(_profileRepositoryMock.Object, _uowMock.Object);
        var command = new CreateProfileCommand("test@email.com", "Password123!", "Test User", UserRole.Farmer);

        var existingProfile = new Profile("test@email.com", "Old User", "hash", UserRole.Farmer);
        _profileRepositoryMock.Setup(repo => repo.FindByEmailAsync(command.Email))
            .ReturnsAsync(existingProfile);

        Assert.ThrowsAsync<InvalidOperationException>(async () => await service.CreateAsync(command));
        
        _profileRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Profile>()), Times.Never);
        _uowMock.Verify(uow => uow.CompleteAsync(), Times.Never);
    }

    [Test]
    public async Task DeleteAsync_ExistingProfile_ShouldRemoveAndReturnTrue()
    {
        var service = new ProfileCommandService(_profileRepositoryMock.Object, _uowMock.Object);
        var command = new DeleteProfileCommand(1);
        
        var profile = new Profile("test@email.com", "User", "hash", UserRole.Farmer);

        _profileRepositoryMock.Setup(repo => repo.FindByIdAsync(command.Id))
            .ReturnsAsync(profile);

        var result = await service.DeleteAsync(command);

        result.Should().BeTrue();
        _profileRepositoryMock.Verify(repo => repo.Remove(profile), Times.Once);
        _uowMock.Verify(uow => uow.CompleteAsync(), Times.Once);
    }

    [Test]
    public async Task ChangePasswordAsync_ValidCurrentPassword_ShouldChangePassword()
    {
        var service = new ProfileCommandService(_profileRepositoryMock.Object, _uowMock.Object);
        
        var profile = new Profile("test@email.com", "User", "hash", UserRole.Farmer);
        var hasher = new PasswordHasher<Profile>();
        profile.SetPasswordHash(hasher.HashPassword(profile, "OldPassword123!"));

        var command = new ChangePasswordCommand(1, "OldPassword123!", "NewPassword123!");

        _profileRepositoryMock.Setup(repo => repo.FindByIdAsync(command.Id))
            .ReturnsAsync(profile);

        var result = await service.ChangePasswordAsync(command);

        result.Should().BeTrue();
        _profileRepositoryMock.Verify(repo => repo.Update(profile), Times.Once);
        _uowMock.Verify(uow => uow.CompleteAsync(), Times.Once);
    }
}
