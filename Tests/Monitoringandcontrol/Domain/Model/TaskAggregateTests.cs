using EcotrackPlatform.API.Monitoringandcontrol.Domain.Model.Aggregates;
using EcotrackPlatform.API.Monitoringandcontrol.Domain.Model.ValueObjects;

namespace EcotrackPlatform.Tests.Monitoringandcontrol.Domain.Model;

[TestFixture]
public class TaskAggregateTests
{
    private const string Title = "Riego de cultivo";
    private const string Description = "Regar el sector A";
    private const int OrganizationId = 1;
    private const int PlotId = 10;
    private const int ResponsibleId = 5;

    [Test]
    public void Constructor_ValidArguments_ShouldCreateTaskInPendingStatus()
    {
        // Act
        var task = new TaskAggregate(Title, Description, OrganizationId, PlotId, ResponsibleId);

        // Assert
        Assert.That(task, Is.Not.Null);
        Assert.That(task.Title, Is.EqualTo(Title));
        Assert.That(task.Description, Is.EqualTo(Description));
        Assert.That(task.OrganizationId, Is.EqualTo(OrganizationId));
        Assert.That(task.PlotId, Is.EqualTo(PlotId));
        Assert.That(task.ResponsibleId, Is.EqualTo(ResponsibleId));
        Assert.That(task.Status, Is.EqualTo(Status.Pending));
        Assert.That(task.CreatedAt, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(1)));
        Assert.That(task.StartedAt, Is.Null);
        Assert.That(task.CompletedAt, Is.Null);
    }

    [Test]
    public void Constructor_NullTitle_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => 
            new TaskAggregate(null!, Description, OrganizationId, PlotId, ResponsibleId));
        
        Assert.That(ex.ParamName, Is.EqualTo("title"));
    }

    [Test]
    public void Start_PendingTask_ShouldSetStatusToInProgressAndSetStartedAt()
    {
        // Arrange
        var task = new TaskAggregate(Title, Description, OrganizationId, PlotId, ResponsibleId);

        // Act
        task.Start();

        // Assert
        Assert.That(task.Status, Is.EqualTo(Status.InProgress));
        Assert.That(task.StartedAt, Is.Not.Null);
        Assert.That(task.StartedAt, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(1)));
        Assert.That(task.UpdatedAt, Is.Not.Null);
    }

    [Test]
    public void Start_AlreadyStartedTask_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var task = new TaskAggregate(Title, Description, OrganizationId, PlotId, ResponsibleId);
        task.Start(); // Status changes to InProgress

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => task.Start());
        
        Assert.That(ex.Message, Does.Contain("Solo se pueden iniciar tareas pendientes"));
    }

    [Test]
    public void Complete_InProgressTask_ShouldSetStatusToCompletedAndSetCompletedAt()
    {
        // Arrange
        var task = new TaskAggregate(Title, Description, OrganizationId, PlotId, ResponsibleId);
        task.Start(); // Need to be InProgress to Complete

        // Act
        task.Complete();

        // Assert
        Assert.That(task.Status, Is.EqualTo(Status.Completed));
        Assert.That(task.CompletedAt, Is.Not.Null);
        Assert.That(task.CompletedAt, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(1)));
        Assert.That(task.UpdatedAt, Is.Not.Null);
    }

    [Test]
    public void Complete_PendingTask_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var task = new TaskAggregate(Title, Description, OrganizationId, PlotId, ResponsibleId);
        // Task is Pending, not InProgress

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => task.Complete());
        
        Assert.That(ex.Message, Does.Contain("Solo se pueden completar tareas que estén en progreso"));
    }

    [Test]
    public void UpdateDetails_ValidArguments_ShouldUpdateProperties()
    {
        // Arrange
        var task = new TaskAggregate(Title, Description, OrganizationId, PlotId, ResponsibleId);
        var newTitle = "Fumigación";
        var newDescription = "Fumigar sector B";
        var newResponsibleId = 8;

        // Act
        task.UpdateDetails(newTitle, newDescription, newResponsibleId);

        // Assert
        Assert.That(task.Title, Is.EqualTo(newTitle));
        Assert.That(task.Description, Is.EqualTo(newDescription));
        Assert.That(task.ResponsibleId, Is.EqualTo(newResponsibleId));
    }

    [Test]
    public void UpdateDetails_NullTitle_ShouldThrowArgumentNullException()
    {
        // Arrange
        var task = new TaskAggregate(Title, Description, OrganizationId, PlotId, ResponsibleId);

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => 
            task.UpdateDetails(null!, "Nueva descripcion", 8));
        
        Assert.That(ex.ParamName, Is.EqualTo("title"));
    }
}
