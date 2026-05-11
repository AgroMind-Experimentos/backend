using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Shared.Domain.Repositories;

namespace EcotrackPlatform.API.Organizations.Domain.Repositories;

public interface IPlotRepository : IBaseRepository<Plot>
{
    Task<IEnumerable<Plot>> ListByOrganizationIdAsync(int organizationId);
}