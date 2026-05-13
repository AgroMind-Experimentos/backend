using EcotrackPlatform.API.Profiles.Domain.Model.ValueObjects;
using EcotrackPlatform.API.Profiles.Domain.Repositories;
using EcotrackPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.EntityFrameworkCore;
using EcotrackPlatform.API.Profiles.Domain.Model.Aggregates;

namespace EcotrackPlatform.API.Profiles.Infrastructure.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly AppDbContext _ctx;
        public ProfileRepository(AppDbContext ctx) => _ctx = ctx;

        public async Task<IEnumerable<Profile>> ListAsync() =>
            await _ctx.Set<Profile>().AsNoTracking().ToListAsync();

        public async Task<IEnumerable<Profile>> ListByRoleAsync(UserRole role) =>
            await _ctx.Set<Profile>().Where(p => p.Role == role).AsNoTracking().ToListAsync();

        public async Task<Profile?> FindByIdAsync(int id) =>
            await _ctx.Set<Profile>().FirstOrDefaultAsync(p => p.Id == id);

        public async Task<Profile?> FindByEmailAsync(string email) =>
            await _ctx.Set<Profile>().FirstOrDefaultAsync(p => p.Email == email);

        public async Task AddAsync(Profile profile) => await _ctx.Set<Profile>().AddAsync(profile);

        public void Update(Profile profile) => _ctx.Set<Profile>().Update(profile);

        public void Remove(Profile profile) => _ctx.Set<Profile>().Remove(profile);
    }
}