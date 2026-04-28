using EcotrackPlatform.API.Monitoringandcontrol.Domain.Model.Aggregates;

namespace EcotrackPlatform.API.Organization.Domain.Model.Aggregates;

public class Crop
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Location { get; private set; }
    public double Area { get; private set; }
    
    public string Cultivation { get; private set; }
    public int OrganizationId { get; private set; }
    
    private readonly List<TaskAggregate> _tasks = new();
    public IReadOnlyCollection<TaskAggregate> Tasks => _tasks.AsReadOnly();
    
    protected Crop() { }

    public Crop(string name, string location, double area, string cultivation, int organizationId)
    {
        Name = name;
        Location = location;
        Area = area;
        Cultivation = cultivation;
        OrganizationId = organizationId;
    }

    public void Update(string name, string location, double area, string cultivation)
    {
        Name = name;
        Location = location;
        Area = area;
        Cultivation = cultivation;
    }
}