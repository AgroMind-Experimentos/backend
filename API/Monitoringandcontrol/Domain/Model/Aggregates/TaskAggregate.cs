using EcotrackPlatform.API.Monitoringandcontrol.Domain.Model.Entities;

namespace EcotrackPlatform.API.Monitoringandcontrol.Domain.Model.Aggregates;

using EcotrackPlatform.API.Monitoringandcontrol.Domain.Model.ValueObjects;

public class TaskAggregate
{
    public int Id { get; private set; }
    public string Title { get; private set; }
    
    public string Description { get; private set; }
    
    public int OrganizationId { get; private set; }
    public int PlotId { get; private set; }
    public int ResponsibleId  { get; private set; }
    public Status Status { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    public Checklist? Checklist { get; private set; }

    private TaskAggregate()
    {
    }

    public TaskAggregate(string title, string description, int organizationId, int plotId, int responsibleId)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description;
        OrganizationId = organizationId;
        PlotId = plotId;
        ResponsibleId = responsibleId;
        Status = Status.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void Start()
    {
        if (Status != Status.Pending)
            throw new InvalidOperationException("Solo se pueden iniciar tareas pendientes.");
            
        Status = Status.InProgress;
        StartedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        if (Status != Status.InProgress)
            throw new InvalidOperationException("Solo se pueden completar tareas que estén en progreso.");
            
        Status = Status.Completed;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string title, string description, int responsibleId)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description;
        ResponsibleId = responsibleId;
    }

    public void SetUpdatedAt()
    {
        UpdatedAt = DateTime.Now;
    }
}