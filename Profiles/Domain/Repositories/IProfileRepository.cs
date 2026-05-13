using EcotrackPlatform.API.Profiles.Domain.Model.ValueObjects;
using EcotrackPlatform.API.Profiles.Domain.Model.Aggregates;

namespace EcotrackPlatform.API.Profiles.Domain.Repositories
{
    public interface IProfileRepository
    {
        Task<IEnumerable<Profile>> ListAsync();
        Task<IEnumerable<Profile>> ListByRoleAsync(UserRole role);
        Task<Profile?> FindByIdAsync(int id);
        Task<Profile?> FindByEmailAsync(string email);
        Task AddAsync(Profile profile);
        void Update(Profile profile);
        void Remove(Profile profile);
    }
}