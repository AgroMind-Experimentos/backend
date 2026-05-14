using EcotrackPlatform.API.Organizations.Domain.Model.Entities;
using EcotrackPlatform.API.Profiles.Domain.Model.Aggregates;

namespace EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;

public class Organization
{
    public int Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public string Location { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private readonly List<OrganizationMember> _members = new();
    public List<OrganizationMember> Members => _members;

    private readonly List<Plot> _plots = new();
    public List<Plot> Plots => _plots;

    public int AgronomistOwnerId { get; private set; }
    public Profile Profile { get; private set; }

    protected Organization() { }

    public Organization(string name, string description, string location, int agronomistOwnerId)
    {
        ValidateString(name, nameof(Name));
        ValidateString(description, nameof(Description));
        ValidateString(location, nameof(Location));

        Name = name;
        Description = description;
        Location = location;
        CreatedAt = DateTime.UtcNow;
        AgronomistOwnerId = agronomistOwnerId;
    }

    public void Update(string? name, string? description, string? location)
    {
        if (name is not null)
        {
            ValidateString(name, nameof(Name));
            Name = name;
        }

        if (description is not null)
        {
            ValidateString(description, nameof(Description));
            Description = description;
        }

        if (location is not null)
        {
            ValidateString(location, nameof(Location));
            Location = location;
        }
    }

    public void SyncMembers(IEnumerable<int> profileIds)
    {
        _members.Clear();

        foreach (var profileId in profileIds.Distinct())
        {
            _members.Add(new OrganizationMember(profileId, Id));
        }
    }

    private static void ValidateString(string value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{propertyName} cannot be empty or whitespace.");
        }
    }
}