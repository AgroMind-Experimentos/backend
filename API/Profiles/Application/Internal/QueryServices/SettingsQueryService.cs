using EcotrackPlatform.API.Profiles.Domain.Repositories;
// Alias para agregado
using ProfileSettingsAgg = EcotrackPlatform.API.Profiles.Domain.Model.Aggregates.ProfileSettings;

namespace EcotrackPlatform.API.Profiles.Application.Internal.QueryServices
{
    public class SettingsQueryService
    {
        private readonly IProfileSettingsRepository _settings;

        public SettingsQueryService(IProfileSettingsRepository settings)
        {
            _settings = settings;
        }

        public Task<ProfileSettingsAgg?> GetByProfileIdAsync(int profileId) =>
            _settings.FindByProfileIdAsync(profileId);
    }
}