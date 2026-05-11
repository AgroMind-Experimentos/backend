using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Shared.Domain.Repositories;

namespace EcotrackPlatform.API.Organizations.Domain.Repositories;

public interface ICropRepository : IBaseRepository<Crop>
{
    Task<IEnumerable<Crop>> FindByOrganizationIdAsync(int organizationId);
    Task<Crop?> FindByIdWithMembersAsync(int id);
    Task<IEnumerable<Crop>> ListWithMembersAsync();
    Task<IEnumerable<Crop>> FindByOrganizationIdWithMembersAsync(int organizationId);
}