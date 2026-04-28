using EcotrackPlatform.API.Organization.Domain.Model.Entities;

namespace EcotrackPlatform.API.Organization.Domain.Model.Aggregates;

public class Organization
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Status { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    
    private readonly List<OrganizationMember> _members = new();
    public IReadOnlyCollection<OrganizationMember> Members => _members.AsReadOnly();
    
    private readonly List<Crop> _crops = new();
    public IReadOnlyCollection<Crop> Crops => _crops.AsReadOnly();
    
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
}