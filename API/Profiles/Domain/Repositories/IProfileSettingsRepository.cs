using ProfileSettingsAgg = EcotrackPlatform.API.Profiles.Domain.Model.Aggregates.ProfileSettings;

namespace EcotrackPlatform.API.Profiles.Domain.Repositories
{
    public interface IProfileSettingsRepository
    {
        Task<ProfileSettingsAgg?> FindByProfileIdAsync(int profileId);
        Task AddAsync(ProfileSettingsAgg settings);
        void Update(ProfileSettingsAgg settings);
    }
}