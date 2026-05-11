using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Profiles.Domain.Model.Aggregates;

namespace EcotrackPlatform.API.Organizations.Domain.Model.Entities;

using Profile = Profile;

public class PlotMember
{
    public int ProfileId { get; private set; }
    public int PlotId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Profile Profile { get; private set; } = null!;
    public Plot Plot { get; private set; } = null!;

    protected PlotMember() { }

    public PlotMember(int profileId, int plotId)
    {
        ProfileId = profileId;
        PlotId = plotId;
        CreatedAt = DateTime.UtcNow;
    }
}