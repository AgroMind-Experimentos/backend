using EcotrackPlatform.API.Organizations.Domain.Model.Entities;
using EcotrackPlatform.API.Organizations.Domain.Repositories;
using EcotrackPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.EntityFrameworkCore;

namespace EcotrackPlatform.API.Organizations.Infrastructure.Repositories;

public class InvitationRepository : IInvitationRepository
{
    private readonly AppDbContext _ctx;
    public InvitationRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task AddAsync(Invitation invitation) => await _ctx.Invitations.AddAsync(invitation);

    public async Task<Invitation?> FindByIdAsync(int id) =>
        await _ctx.Invitations.FirstOrDefaultAsync(i => i.Id == id);

    public async Task<IEnumerable<Invitation>> FindPendingByFarmerAsync(int farmerProfileId) =>
        await _ctx.Invitations
            .Where(i => i.FarmerProfileId == farmerProfileId && i.Status == InvitationStatus.Pending)
            .ToListAsync();

    public async Task<bool> ExistsAsync(int organizationId, int farmerProfileId) =>
        await _ctx.Invitations.AnyAsync(i =>
            i.OrganizationId == organizationId &&
            i.FarmerProfileId == farmerProfileId &&
            i.Status == InvitationStatus.Pending);

    public void Update(Invitation invitation) => _ctx.Invitations.Update(invitation);
}
