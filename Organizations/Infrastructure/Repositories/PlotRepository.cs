using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Organizations.Domain.Repositories;
using EcotrackPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.EntityFrameworkCore;

namespace EcotrackPlatform.API.Organizations.Infrastructure.Repositories;

public class PlotRepository(AppDbContext context) : IPlotRepository
{
    public async Task AddAsync(Plot entity) =>
        await context.Plots.AddAsync(entity);

    public async Task<Plot?> FindByIdAsync(int id) =>
        await context.Plots.FindAsync(id);

    public async Task<IEnumerable<Plot>> ListAsync() =>
        await context.Plots.ToListAsync();

    public async Task<IEnumerable<Plot>> ListByOrganizationIdAsync(int organizationId) =>
        await context.Plots.Where(c => c.OrganizationId == organizationId).ToListAsync();

    public void Remove(Plot entity) => context.Plots.Remove(entity);

    public void Update(Plot entity) => context.Plots.Update(entity);
}