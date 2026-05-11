using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Shared.Domain.Repositories;

namespace EcotrackPlatform.API.Organizations.Domain.Repositories;

public interface IPlotRepository : IBaseRepository<Plot>
{
    Task<IEnumerable<Plot>> FindByOrganizationIdAsync(int organizationId);
    Task<Plot?> FindByIdWithMembersAsync(int id);
    Task<IEnumerable<Plot>> ListWithMembersAsync();
    Task<IEnumerable<Plot>> FindByOrganizationIdWithMembersAsync(int organizationId);
}