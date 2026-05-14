using EcotrackPlatform.API.Profiles.Domain.Model.Aggregates;

namespace EcotrackPlatform.API.Iam.Domain.Services;

public interface ITokenService
{
    string GenerateToken(Profile profile);
}
