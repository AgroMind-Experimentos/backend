using EcotrackPlatform.API.Monitoringandcontrol.Domain.Model.Aggregates;
using FluentAssertions;
using NUnit.Framework;
using TaskStatus = EcotrackPlatform.API.Monitoringandcontrol.Domain.Model.ValueObjects.TaskStatus;

namespace EcotrackPlatform.API.Tests;

[TestFixture]
public class TaskAggregateTests
{
    [Test]
    public void CreateTask_WithValidData_ShouldAssignResponsibleCorrectly()
    {
        // Arrange
        var title = "Cosecha de Café";
        var description = "Recolección de granos maduros en lote norte";
        var orgId = 1;
        var plotId = 10;
        var responsibleId = 5;

        // Act
        var task = new TaskAggregate(title, description, orgId, plotId, responsibleId);

        // Assert
        task.Title.Should().Be(title);
        task.ResponsibleId.Should().Be(responsibleId);
        task.Status.Should().Be(TaskStatus.Pending);
        task.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Test]
    public void CreateTask_WithEmptyTitle_ShouldThrowArgumentNullException()
    {
        // Arrange
        string? invalidTitle = null;

        // Act
        Action act = () => new TaskAggregate(invalidTitle!, "Desc", 1, 1, 1);

        // Assert
        act.Should().Throw<ArgumentNullException>()
           .WithParameterName("title");
    }
    
    [Test]
    public void UpdateDetails_WithValidData_ShouldUpdateCorrectly()
    {
        // Arrange
        var task = new TaskAggregate("Tarea Vieja", "Desc Vieja", 1, 1, 10);
        var newTitle = "Tarea Actualizada";
        var newDescription = "Nueva descripción técnica";
        var newResponsibleId = 20;

        // Act
        task.UpdateDetails(newTitle, newDescription, newResponsibleId);
        task.SetUpdatedAt();

        // Assert
        task.Title.Should().Be(newTitle);
        task.Description.Should().Be(newDescription);
        task.ResponsibleId.Should().Be(newResponsibleId);
        task.UpdatedAt.Should().NotBeNull();
        task.UpdatedAt.Value.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
    }
    
    [Test]
    public void UpdateDetails_WithNullTitle_ShouldThrowExceptionAndKeepOriginalData()
    {
        // Arrange
        var originalTitle = "Título Intocable";
        var task = new TaskAggregate(originalTitle, "Descripción", 1, 1, 10);

        // Act
        Action act = () => task.UpdateDetails(null!, "Nueva Desc", 20);

        // Assert
        act.Should().Throw<ArgumentNullException>();
        task.Title.Should().Be(originalTitle);
    }
    
    [Test]
    public void Complete_WhenTaskIsInProgress_ShouldSetStatusToCompleted()
    {
        // Arrange
        var task = new TaskAggregate("Cosecha", "Lote 1", 1, 10, 5);
        task.Start();
        
        // Act
        task.Complete();

        // Assert
        task.Status.Should().Be(TaskStatus.Completed);
        task.CompletedAt.Should().NotBeNull();
        task.CompletedAt.Value.Date.Should().Be(DateTime.UtcNow.Date);
        task.UpdatedAt.Should().NotBeNull();
    }

    [Test]
    public void Complete_WhenTaskIsPending_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var task = new TaskAggregate("Fumigación", "Lote 2", 1, 10, 5);

        // Act
        Action act = () => task.Complete();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Solo se pueden completar tareas que estén en progreso.");
        task.Status.Should().Be(TaskStatus.Pending);
    }
}