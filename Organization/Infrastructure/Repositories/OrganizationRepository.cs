using EcotrackPlatform.API.Organization.Domain.Repositories;
using EcotrackPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using EcotrackPlatform.API.Shared.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EcotrackPlatform.API.Organization.Infrastructure.Repositories;

public class OrganizationRepository(AppDbContext context)
    : BaseRepository<Domain.Model.Aggregates.Organization>(context), IOrganizationRepository
{
    public async Task<Domain.Model.Aggregates.Organization?> FindByIdWithMembersAsync(int id) =>
        await Context.Set<Domain.Model.Aggregates.Organization>()
            .Include(org => org.Members)
            .FirstOrDefaultAsync(org => org.Id == id);

    public async Task<IEnumerable<Domain.Model.Aggregates.Organization>> ListWithMembersAsync() =>
        await Context.Set<Domain.Model.Aggregates.Organization>()
            .Include(org => org.Members)
            .ToListAsync();
}