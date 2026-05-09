namespace EcotrackPlatform.API.Organization.Domain.Model.Entities;

public enum InvitationStatus { Pending, Accepted, Rejected }

public class Invitation
{
    public int Id { get; private set; }
    public int OrganizationId { get; private set; }
    public int FarmerProfileId { get; private set; }
    public int AgronomistProfileId { get; private set; }
    public InvitationStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    protected Invitation() { }

    public Invitation(int organizationId, int farmerProfileId, int agronomistProfileId)
    {
        OrganizationId = organizationId;
        FarmerProfileId = farmerProfileId;
        AgronomistProfileId = agronomistProfileId;
        Status = InvitationStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void Accept() => Status = InvitationStatus.Accepted;
    public void Reject() => Status = InvitationStatus.Rejected;
}
