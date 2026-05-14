using System;
using System.Threading.Tasks;
using EcotrackPlatform.API.Monitoringandcontrol.Application.Internal.CommandServices;
using EcotrackPlatform.API.Monitoringandcontrol.Domain.Model.Aggregates;
using EcotrackPlatform.API.Monitoringandcontrol.Domain.Repositories;
using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Organizations.Domain.Repositories;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace EcotrackPlatform.Tests.Monitoringandcontrol.Application.Internal.CommandServices;

[TestFixture]
public class CreateTaskCommandServiceTests
{
    private Mock<ITaskRepository> _taskRepositoryMock;
    private Mock<IPlotRepository> _plotRepositoryMock;
    private Mock<IOrganizationRepository> _orgRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _taskRepositoryMock = new Mock<ITaskRepository>();
        _plotRepositoryMock = new Mock<IPlotRepository>();
        _orgRepositoryMock = new Mock<IOrganizationRepository>();
    }

    [Test]
    public async Task Handle_ValidData_ShouldCreateTaskAndReturnId()
    {
        var service = new CreateTaskCommandService(
            _taskRepositoryMock.Object, 
            _plotRepositoryMock.Object, 
            _orgRepositoryMock.Object);

        var title = "Riego";
        var description = "Regar el maíz";
        var orgId = 1;
        var plotId = 10;
        var responsibleId = 5;
        var expectedTaskId = 100;

        var org = new Organization("Org", "Desc", "Loc", 5);
        var plot = new Plot("Plot", "Loc", 10.0, "Crop", orgId);

        _orgRepositoryMock.Setup(repo => repo.FindByIdAsync(orgId)).ReturnsAsync(org);
        _plotRepositoryMock.Setup(repo => repo.FindByIdAsync(plotId)).ReturnsAsync(plot);
        
        _taskRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<TaskAggregate>()))
            .ReturnsAsync(expectedTaskId);

        var result = await service.Handle(title, description, orgId, plotId, responsibleId);

        result.Should().Be(expectedTaskId);
        _taskRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<TaskAggregate>()), Times.Once);
    }

    [Test]
    public void Handle_OrganizationNotFound_ShouldThrowInvalidOperationException()
    {
        var service = new CreateTaskCommandService(
            _taskRepositoryMock.Object, 
            _plotRepositoryMock.Object, 
            _orgRepositoryMock.Object);

        var orgId = 99;

        _orgRepositoryMock.Setup(repo => repo.FindByIdAsync(orgId)).ReturnsAsync((Organization?)null);

        Assert.ThrowsAsync<InvalidOperationException>(async () => 
            await service.Handle("Title", "Desc", orgId, 1, 1));
            
        _taskRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<TaskAggregate>()), Times.Never);
    }

    [Test]
    public void Handle_PlotNotFound_ShouldThrowInvalidOperationException()
    {
        var service = new CreateTaskCommandService(
            _taskRepositoryMock.Object, 
            _plotRepositoryMock.Object, 
            _orgRepositoryMock.Object);

        var orgId = 1;
        var plotId = 99;

        var org = new Organization("Org", "Desc", "Loc", 5);

        _orgRepositoryMock.Setup(repo => repo.FindByIdAsync(orgId)).ReturnsAsync(org);
        _plotRepositoryMock.Setup(repo => repo.FindByIdAsync(plotId)).ReturnsAsync((Plot?)null);

        Assert.ThrowsAsync<InvalidOperationException>(async () => 
            await service.Handle("Title", "Desc", orgId, plotId, 1));
            
        _taskRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<TaskAggregate>()), Times.Never);
    }
}
