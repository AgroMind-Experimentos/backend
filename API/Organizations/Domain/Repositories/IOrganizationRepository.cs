using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Shared.Domain.Repositories;

 namespace EcotrackPlatform.API.Organizations.Domain.Repositories;

public interface IOrganizationRepository : IBaseRepository<Organization>
{
	Task<Organization?> FindByIdWithMembersAsync(int id);
	Task<IEnumerable<Organization>> ListWithMembersAsync();
	Task<IEnumerable<Organization>> ListByMemberAsync(int profileId);
}