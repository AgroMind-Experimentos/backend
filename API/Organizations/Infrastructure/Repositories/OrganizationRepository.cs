using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Organizations.Domain.Repositories;
using EcotrackPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using EcotrackPlatform.API.Shared.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EcotrackPlatform.API.Organizations.Infrastructure.Repositories;

public class OrganizationRepository(AppDbContext context)
    : BaseRepository<Organization>(context), IOrganizationRepository
{
    public async Task<Organization?> FindByIdWithMembersAsync(int id) =>
        await Context.Set<Organization>()
            .Include(org => org.Members)
            .FirstOrDefaultAsync(org => org.Id == id);

    public async Task<IEnumerable<Organization>> ListWithMembersAsync() =>
        await Context.Set<Organization>()
            .Include(org => org.Members)
            .ToListAsync();

    public async Task<IEnumerable<Organization>> ListByMemberAsync(int profileId) =>
        await Context.Set<Organization>()
            .Include(org => org.Members)
            .Where(org => org.Members.Any(m => m.ProfileId == profileId))
            .ToListAsync();
}