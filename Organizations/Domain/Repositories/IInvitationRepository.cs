using EcotrackPlatform.API.Organizations.Domain.Model.Entities;

namespace EcotrackPlatform.API.Organizations.Domain.Repositories;

public interface IInvitationRepository
{
    Task AddAsync(Invitation invitation);
    Task<Invitation?> FindByIdAsync(int id);
    Task<IEnumerable<Invitation>> FindPendingByFarmerAsync(int farmerProfileId);
    Task<IEnumerable<Invitation>> FindPendingByOrganizationAsync(int organizationId);
    Task<bool> ExistsAsync(int organizationId, int farmerProfileId);
    void Update(Invitation invitation);
}
