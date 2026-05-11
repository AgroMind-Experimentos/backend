using EcotrackPlatform.API.Organizations.Application.Services;
using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Organizations.Domain.Model.Commands;
using EcotrackPlatform.API.Organizations.Domain.Repositories;
using EcotrackPlatform.API.Profiles.Domain.Repositories;
using EcotrackPlatform.API.Shared.Domain.Repositories;

namespace EcotrackPlatform.API.Organizations.Application.Internal.CommandServices;

public class OrganizationCommandService(
    IOrganizationRepository repository,
    IProfileRepository profileRepository,
    IInvitationRepository invitationRepository,
    IUnitOfWork unitOfWork)
    : IOrganizationCommandService
{
    public async Task<Organization> Handle(CreateOrganizationCommand command)
    {
        var org = new Organization(
            command.Name,
            command.Description,
            command.Location
        );

        await repository.AddAsync(org);
        await unitOfWork.CompleteAsync();

        // Add only the agronomist as the initial member
        if (command.AgronomistId.HasValue)
        {
            org.SyncMembers(new[] { command.AgronomistId.Value });
            repository.Update(org);
            await unitOfWork.CompleteAsync();
        }

        return org;
    }

    public async Task<Organization?> UpdateAsync(UpdateOrganizationCommand command)
    {
        var organization = await repository.FindByIdWithMembersAsync(command.Id);
        if (organization is null) return null;

        organization.Update(
            organization.Name,
            organization.Description,
            organization.Location
        );

        var memberIds = command.MemberIds;
        if (memberIds is not null)
        {
            await ValidateProfilesExistAsync(memberIds);
            organization.SyncMembers(memberIds);
        }

        repository.Update(organization);
        await unitOfWork.CompleteAsync();
        return organization;
    }

    public async Task<bool> Handle(DeleteOrganizationByIdCommand command)
    {
        var organization = await repository.FindByIdAsync(command.Id);
        if (organization is null) return false;

        var pendingInvitations = await invitationRepository.FindPendingByOrganizationAsync(command.Id);
        foreach (var invitation in pendingInvitations)
        {
            invitation.Cancel();
            invitationRepository.Update(invitation);
        }

        repository.Remove(organization);
        await unitOfWork.CompleteAsync();
        return true;
    }

    private async Task ValidateProfilesExistAsync(IEnumerable<int> profileIds)
    {
        foreach (var profileId in profileIds.Distinct())
        {
            var profile = await profileRepository.FindByIdAsync(profileId);
            if (profile is null)
            {
                throw new InvalidOperationException($"Profile with id {profileId} does not exist.");
            }
        }
    }
}