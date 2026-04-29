using EcotrackPlatform.API.Shared.Domain.Repositories;
using EcotrackPlatform.API.Organization.Domain.Model.Aggregates;

namespace EcotrackPlatform.API.Organization.Domain.Repositories;

public interface ICropRepository : IBaseRepository<Crop>
{
    Task<IEnumerable<Crop>> FindByOrganizationIdAsync(int organizationId);
    Task<Crop?> FindByIdWithMembersAsync(int id);
    Task<IEnumerable<Crop>> ListWithMembersAsync();
    Task<IEnumerable<Crop>> FindByOrganizationIdWithMembersAsync(int organizationId);
}