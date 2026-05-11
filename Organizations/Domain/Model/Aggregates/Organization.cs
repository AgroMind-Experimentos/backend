using EcotrackPlatform.API.Organizations.Domain.Model.Entities;

namespace EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;

public class Organization
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Status { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    
    private readonly List<OrganizationMember> _members = new();
    public List<OrganizationMember> Members => _members;
    
    private readonly List<Crop> _crops = new();
    public List<Crop> Crops => _crops;
    
    protected Organization() { }
    
    public Organization(string name, string description, string status)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        Status = status;
        CreatedAt = DateTime.UtcNow;
    }
    
    public void Update(string name, string description, string status)
    {
        Name = name.Trim();
        Description = description.Trim();
        Status = status.Trim();
    }

    public void SyncMembers(IEnumerable<int> profileIds)
    {
        _members.Clear();

        foreach (var profileId in profileIds.Distinct())
        {
            _members.Add(new OrganizationMember(profileId, Id));
        }
    }

    public bool HasMember(int profileId) => _members.Any(member => member.ProfileId == profileId);
}