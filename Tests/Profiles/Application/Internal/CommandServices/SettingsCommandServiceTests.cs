using EcotrackPlatform.API.Profiles.Application.Internal.CommandServices;
using EcotrackPlatform.API.Profiles.Domain.Model.Aggregates;
using EcotrackPlatform.API.Profiles.Domain.Repositories;
using EcotrackPlatform.API.Shared.Domain.Repositories;

namespace EcotrackPlatform.Tests.Profiles.Application.Internal.CommandServices;

[TestFixture]
public class SettingsCommandServiceTests
{
    private Mock<IProfileSettingsRepository> _settingsRepositoryMock;
    private Mock<IUnitOfWork> _uowMock;

    [SetUp]
    public void Setup()
    {
        _settingsRepositoryMock = new Mock<IProfileSettingsRepository>();
        _uowMock = new Mock<IUnitOfWork>();
    }

    [Test]
    public async Task UpsertAsync_SettingsNotExist_ShouldCreateNewSettings()
    {
        var service = new SettingsCommandService(_settingsRepositoryMock.Object, _uowMock.Object);
        var profileId = 1;

        _settingsRepositoryMock.Setup(repo => repo.FindByProfileIdAsync(profileId))
            .ReturnsAsync((ProfileSettings?)null);

        var result = await service.UpsertAsync(profileId, true, "es", "dark");

        result.Should().NotBeNull();
        
        // Settings exist verify AddAsync is called
        _settingsRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<ProfileSettings>()), Times.Once);
        _settingsRepositoryMock.Verify(repo => repo.Update(It.IsAny<ProfileSettings>()), Times.Never);
        _uowMock.Verify(uow => uow.CompleteAsync(), Times.Once);
    }

    [Test]
    public async Task UpsertAsync_SettingsExist_ShouldUpdatePropertiesUsingReflection()
    {
        var service = new SettingsCommandService(_settingsRepositoryMock.Object, _uowMock.Object);
        var profileId = 1;
        
        var existingSettings = new ProfileSettings(profileId, false, "en", "light");

        _settingsRepositoryMock.Setup(repo => repo.FindByProfileIdAsync(profileId))
            .ReturnsAsync(existingSettings);

        var result = await service.UpsertAsync(profileId, true, "es", "dark");

        result.Should().NotBeNull();
        
        // Validate update happened
        _settingsRepositoryMock.Verify(repo => repo.Update(existingSettings), Times.Once);
        _settingsRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<ProfileSettings>()), Times.Never);
        _uowMock.Verify(uow => uow.CompleteAsync(), Times.Once);
    }
}
