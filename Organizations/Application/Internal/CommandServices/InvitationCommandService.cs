using EcotrackPlatform.API.Organizations.Domain.Model.Entities;
using EcotrackPlatform.API.Organizations.Domain.Repositories;
using EcotrackPlatform.API.Profiles.Domain.Repositories;
using EcotrackPlatform.API.Shared.Domain.Repositories;

namespace EcotrackPlatform.API.Organizations.Application.Internal.CommandServices;

public enum InviteResult { Success, InvalidEmail, UserNotFound, DuplicatePending }

public class InvitationCommandService(
    IInvitationRepository invitations,
    IOrganizationRepository organizations,
    IProfileRepository profiles,
    IUnitOfWork uow)
{
    public async Task<InviteResult> SendInvitationByEmailAsync(int orgId, string email, int agronomistId)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@') || !email.Contains('.'))
            return InviteResult.InvalidEmail;

        var farmer = await profiles.FindByEmailAsync(email.Trim().ToLower());
        if (farmer is null)
            return InviteResult.UserNotFound;

        if (await invitations.ExistsAsync(orgId, farmer.Id))
            return InviteResult.DuplicatePending;

        await invitations.AddAsync(new Invitation(orgId, farmer.Id, agronomistId));
        await uow.CompleteAsync();
        return InviteResult.Success;
    }

    public async Task<bool> AcceptAsync(int invitationId, int farmerProfileId)
    {
        var invitation = await invitations.FindByIdAsync(invitationId);
        if (invitation is null || invitation.FarmerProfileId != farmerProfileId || invitation.Status != InvitationStatus.Pending)
            return false;

        var org = await organizations.FindByIdWithMembersAsync(invitation.OrganizationId);
        if (org is null) return false;

        var memberIds = org.Members.Select(m => m.ProfileId).ToList();
        if (!memberIds.Contains(farmerProfileId))
        {
            memberIds.Add(farmerProfileId);
            org.SyncMembers(memberIds);
            organizations.Update(org);
        }

        invitation.Accept();
        invitations.Update(invitation);
        await uow.CompleteAsync();
        return true;
    }

    public async Task<bool> RejectAsync(int invitationId, int farmerProfileId)
    {
        var invitation = await invitations.FindByIdAsync(invitationId);
        if (invitation is null || invitation.FarmerProfileId != farmerProfileId || invitation.Status != InvitationStatus.Pending)
            return false;

        invitation.Reject();
        invitations.Update(invitation);
        await uow.CompleteAsync();
        return true;
    }

    public async Task<IEnumerable<Invitation>> GetPendingForFarmerAsync(int farmerProfileId) =>
        await invitations.FindPendingByFarmerAsync(farmerProfileId);
}
