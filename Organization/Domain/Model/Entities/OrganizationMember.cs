using EcotrackPlatform.API.Profiles.Domain.Model.Aggregates;

namespace EcotrackPlatform.API.Organization.Domain.Model.Entities;

public class OrganizationMember
{
    public int ProfileId { get; private set; }
    public int OrganizationId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    public Profile Profile { get; private set; } = null!;
    public Aggregates.Organization Organization { get; private set; } = null!;
    
    protected OrganizationMember() { }

    public OrganizationMember(int profileId, int organizationId)
    {
        ProfileId = profileId;
        OrganizationId = organizationId;
        CreatedAt = DateTime.UtcNow;
    }
}