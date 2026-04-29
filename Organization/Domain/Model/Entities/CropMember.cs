namespace EcotrackPlatform.API.Organization.Domain.Model.Entities;

using EcotrackPlatform.API.Organization.Domain.Model.Aggregates;
using ProfileAgg = EcotrackPlatform.API.Profile.Domain.Model.Aggregates.Profile;

public class CropMember
{
    public int ProfileId { get; private set; }
    public int CropId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public ProfileAgg Profile { get; private set; } = null!;
    public Crop Crop { get; private set; } = null!;

    protected CropMember() { }

    public CropMember(int profileId, int cropId)
    {
        ProfileId = profileId;
        CropId = cropId;
        CreatedAt = DateTime.UtcNow;
    }
}