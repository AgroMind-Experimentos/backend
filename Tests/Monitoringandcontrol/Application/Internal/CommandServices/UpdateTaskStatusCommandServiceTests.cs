using System.Collections.Generic;
using System.Threading.Tasks;
using EcotrackPlatform.API.Monitoringandcontrol.Application.Internal.CommandServices;
using EcotrackPlatform.API.Monitoringandcontrol.Domain.Model.Aggregates;
using EcotrackPlatform.API.Monitoringandcontrol.Domain.Model.ValueObjects;
using EcotrackPlatform.API.Monitoringandcontrol.Domain.Repositories;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace EcotrackPlatform.Tests.Monitoringandcontrol.Application.Internal.CommandServices;

[TestFixture]
public class UpdateTaskStatusCommandServiceTests
{
    private Mock<ITaskRepository> _taskRepositoryMock;
    private Mock<IChecklistRepository> _checklistRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _taskRepositoryMock = new Mock<ITaskRepository>();
        _checklistRepositoryMock = new Mock<IChecklistRepository>();
    }

    [Test]
    public async Task Handle_StartTask_ShouldChangeStatusToInProgressAndSave()
    {
        var service = new UpdateTaskStatusCommandService(_taskRepositoryMock.Object, _checklistRepositoryMock.Object);
        var taskId = 1;
        
        var task = new TaskAggregate("Task", "Desc", 1, 1, 1); // Status defaults to Pending
        
        _taskRepositoryMock.Setup(repo => repo.GetByIdAsync(taskId)).ReturnsAsync(task);

        await service.Handle(taskId, "InProgress");

        task.Status.Should().Be(Status.InProgress);
        _taskRepositoryMock.Verify(repo => repo.UpdateAsync(task), Times.Once);
    }

    [Test]
    public async Task Handle_CompleteTask_ShouldChangeStatusToCompletedAndSave()
    {
        var service = new UpdateTaskStatusCommandService(_taskRepositoryMock.Object, _checklistRepositoryMock.Object);
        var taskId = 1;
        
        var task = new TaskAggregate("Task", "Desc", 1, 1, 1);
        task.Start(); // Mutate to InProgress so it can be completed
        
        _taskRepositoryMock.Setup(repo => repo.GetByIdAsync(taskId)).ReturnsAsync(task);

        await service.Handle(taskId, "Completed");

        task.Status.Should().Be(Status.Completed);
        _taskRepositoryMock.Verify(repo => repo.UpdateAsync(task), Times.Once);
    }

    [Test]
    public void Handle_TaskNotFound_ShouldThrowKeyNotFoundException()
    {
        var service = new UpdateTaskStatusCommandService(_taskRepositoryMock.Object, _checklistRepositoryMock.Object);
        var taskId = 99;
        
        _taskRepositoryMock.Setup(repo => repo.GetByIdAsync(taskId)).ReturnsAsync((TaskAggregate?)null);

        Assert.ThrowsAsync<KeyNotFoundException>(async () => await service.Handle(taskId, "InProgress"));
        
        _taskRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<TaskAggregate>()), Times.Never);
    }
}
