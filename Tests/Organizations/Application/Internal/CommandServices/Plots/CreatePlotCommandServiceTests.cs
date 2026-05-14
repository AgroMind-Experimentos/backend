using EcotrackPlatform.API.Organizations.Application.Internal.CommandServices.Plots;
using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Organizations.Domain.Model.Commands;
using EcotrackPlatform.API.Organizations.Domain.Repositories;
using EcotrackPlatform.API.Shared.Domain.Repositories;

namespace EcotrackPlatform.Tests.Organizations.Application.Internal.CommandServices.Plots;

[TestFixture]
public class CreatePlotCommandServiceTests
{
    private Mock<IPlotRepository> _plotRepositoryMock;
    private Mock<IOrganizationRepository> _orgRepositoryMock;
    private Mock<IUnitOfWork> _uowMock;

    [SetUp]
    public void Setup()
    {
        _plotRepositoryMock = new Mock<IPlotRepository>();
        _orgRepositoryMock = new Mock<IOrganizationRepository>();
        _uowMock = new Mock<IUnitOfWork>();
    }

    [Test]
    public async Task CreateAsync_ValidCommand_ShouldReturnSuccess()
    {
        var service = new CreatePlotCommandService(_plotRepositoryMock.Object, _orgRepositoryMock.Object, _uowMock.Object);
        var orgId = 1;
        var command = new CreatePlotCommand("Lote 1", "Norte", 15.5, "Trigo", orgId);
        
        var organization = new Organization("Org", "Desc", "Loc", 5);

        _orgRepositoryMock.Setup(repo => repo.FindByIdWithMembersAsync(orgId)).ReturnsAsync(organization);

        var result = await service.CreateAsync(command);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Plot, Is.Not.Null);
        Assert.That(result.Plot!.Name, Is.EqualTo("Lote 1"));
        
        _plotRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Plot>()), Times.Once);
        _uowMock.Verify(uow => uow.CompleteAsync(), Times.Once);
    }

    [Test]
    public async Task CreateAsync_OrganizationNotFound_ShouldReturnError()
    {
        var service = new CreatePlotCommandService(_plotRepositoryMock.Object, _orgRepositoryMock.Object, _uowMock.Object);
        var command = new CreatePlotCommand("Lote 1", "Norte", 15.5, "Trigo", 99);

        _orgRepositoryMock.Setup(repo => repo.FindByIdWithMembersAsync(99)).ReturnsAsync((Organization?)null);

        var result = await service.CreateAsync(command);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.EqualTo(CreatePlotError.OrganizationNotFound));
        
        _plotRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Plot>()), Times.Never);
        _uowMock.Verify(uow => uow.CompleteAsync(), Times.Never);
    }

    [Test]
    public async Task CreateAsync_InvalidPlotData_ShouldCatchExceptionAndReturnError()
    {
        var service = new CreatePlotCommandService(_plotRepositoryMock.Object, _orgRepositoryMock.Object, _uowMock.Object);
        var orgId = 1;
        // Area 0 will cause an ArgumentException from the Plot constructor
        var command = new CreatePlotCommand("Lote 1", "Norte", 0, "Trigo", orgId);
        
        var organization = new Organization("Org", "Desc", "Loc", 5);

        _orgRepositoryMock.Setup(repo => repo.FindByIdWithMembersAsync(orgId)).ReturnsAsync(organization);

        var result = await service.CreateAsync(command);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.EqualTo(CreatePlotError.InvalidPlotData));
        
        _plotRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Plot>()), Times.Never);
        _uowMock.Verify(uow => uow.CompleteAsync(), Times.Never);
    }
}
