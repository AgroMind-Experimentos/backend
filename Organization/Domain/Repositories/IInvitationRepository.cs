using EcotrackPlatform.API.Organization.Domain.Model.Entities;

namespace EcotrackPlatform.API.Organization.Domain.Repositories;

public interface IInvitationRepository
{
    Task AddAsync(Invitation invitation);
    Task<Invitation?> FindByIdAsync(int id);
    Task<IEnumerable<Invitation>> FindPendingByFarmerAsync(int farmerProfileId);
    Task<bool> ExistsAsync(int organizationId, int farmerProfileId);
    void Update(Invitation invitation);
}
