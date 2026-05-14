using EcotrackPlatform.API.Profiles.Domain.Model.Aggregates;
using EcotrackPlatform.API.Profiles.Domain.Repositories;
using EcotrackPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.EntityFrameworkCore;

namespace EcotrackPlatform.API.Profiles.Infrastructure.Repositories
{
    public class ProfileSettingsRepository : IProfileSettingsRepository
    {
        private readonly AppDbContext _ctx;
        public ProfileSettingsRepository(AppDbContext ctx) => _ctx = ctx;

        public async Task<ProfileSettings?> FindByProfileIdAsync(int profileId) =>
            await _ctx.Set<ProfileSettings>().AsNoTracking().FirstOrDefaultAsync(s => s.ProfileId == profileId);

        public async Task AddAsync(ProfileSettings settings) =>
            await _ctx.Set<ProfileSettings>().AddAsync(settings);

        public void Update(ProfileSettings settings) =>
            _ctx.Set<ProfileSettings>().Update(settings);
    }
}