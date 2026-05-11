using EcotrackPlatform.API.Monitoringandcontrol.Domain.Model.Aggregates;
using EcotrackPlatform.API.Organizations.Domain.Model.Entities;

namespace EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;

public class Plot
{
    public int Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string Location { get; private set; } = default!;
    public double Area { get; private set; }
    
    public string Cultivation { get; private set; } = default!;
    public int OrganizationId { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    
    private readonly List<TaskAggregate> _tasks = new();
    public List<TaskAggregate> Tasks => _tasks;
    
    protected Plot() { }

    public Plot(string name, string location, double area, string cultivation, int organizationId)
    {
        Name = name;
        Location = location;
        Area = area;
        Cultivation = cultivation;
        OrganizationId = organizationId;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string name, string location, double area, string cultivation)
    {
        Name = name;
        Location = location;
        Area = area;
        Cultivation = cultivation;
    }

    private readonly List<PlotMember> _members = new();
    public List<PlotMember> Members => _members;

    public void SyncMembers(IEnumerable<int> profileIds)
    {
        _members.Clear();

        foreach (var profileId in profileIds.Distinct())
        {
            _members.Add(new PlotMember(profileId, Id));
        }
    }

    public bool HasMember(int profileId) => _members.Any(member => member.ProfileId == profileId);
}