using EcotrackPlatform.API.Organizations.Application.Internal.CommandServices.Organizations;
using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Organizations.Domain.Model.Commands;
using EcotrackPlatform.API.Organizations.Domain.Repositories;
using EcotrackPlatform.API.Shared.Domain.Repositories;

namespace EcotrackPlatform.Tests.Organizations.Application.Internal.CommandServices.Organizations;

[TestFixture]
public class CreateOrganizationCommandServiceTests
{
    private Mock<IOrganizationRepository> _repositoryMock;
    private Mock<IUnitOfWork> _uowMock;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<IOrganizationRepository>();
        _uowMock = new Mock<IUnitOfWork>();
    }

    [Test]
    public async Task CreateAsync_ValidCommand_ShouldCreateOrganizationAndSyncMembers()
    {
        var service = new CreateOrganizationCommandService(_repositoryMock.Object, _uowMock.Object);
        var command = new CreateOrganizationCommand("Finca Nueva", "Desc", "Loc", 5);

        var result = await service.CreateAsync(command);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Organization, Is.Not.Null);
        Assert.That(result.Organization!.Name, Is.EqualTo("Finca Nueva"));
        Assert.That(result.Organization.AgronomistOwnerId, Is.EqualTo(5));

        _repositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Organization>()), Times.Once);
        _repositoryMock.Verify(repo => repo.Update(It.IsAny<Organization>()), Times.Once);
        _uowMock.Verify(uow => uow.CompleteAsync(), Times.Exactly(2));
    }

    [Test]
    public async Task CreateAsync_InvalidData_ShouldCatchExceptionAndReturnError()
    {
        var service = new CreateOrganizationCommandService(_repositoryMock.Object, _uowMock.Object);
        var command = new CreateOrganizationCommand("", "Desc", "Loc", 5);

        var result = await service.CreateAsync(command);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.EqualTo(CreateOrganizationError.InvalidOrganizationData));
        
        _repositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Organization>()), Times.Never);
        _uowMock.Verify(uow => uow.CompleteAsync(), Times.Never);
    }
}
