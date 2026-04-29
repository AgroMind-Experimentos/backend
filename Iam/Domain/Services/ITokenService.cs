using ProfileAgg = EcotrackPlatform.API.Profile.Domain.Model.Aggregates.Profile;

namespace EcotrackPlatform.API.Iam.Domain.Services;

public interface ITokenService
{
    string GenerateToken(ProfileAgg profile);
}
