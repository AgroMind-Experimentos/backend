using EcotrackPlatform.API.Profiles.Domain.Model.ValueObjects;
using EcotrackPlatform.API.Profiles.Domain.Repositories;
using EcotrackPlatform.API.Profiles.Domain.Model.Aggregates;

namespace EcotrackPlatform.API.Profiles.Application.Internal.QueryServices
{
    public class ProfileQueryService
    {
        private readonly IProfileRepository _profiles;

        public ProfileQueryService(IProfileRepository profiles)
        {
            _profiles = profiles;
        }

        public Task<IEnumerable<Profile>> ListAsync() => _profiles.ListAsync();
        public Task<IEnumerable<Profile>> ListByRoleAsync(UserRole role) => _profiles.ListByRoleAsync(role);
        public Task<Profile?> FindByIdAsync(int id) => _profiles.FindByIdAsync(id);
        public Task<Profile?> FindByEmailAsync(string email) => _profiles.FindByEmailAsync(email);
        public Task<Profile?> GetCurrentAsync(int currentProfileId) => _profiles.FindByIdAsync(currentProfileId);
    }
}