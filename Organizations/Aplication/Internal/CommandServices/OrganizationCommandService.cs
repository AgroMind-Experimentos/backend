using EcotrackPlatform.API.Organizations.Aplication.Services;
using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Organizations.Domain.Model.Commands;
using EcotrackPlatform.API.Organizations.Domain.Repositories;
using EcotrackPlatform.API.Profiles.Domain.Repositories;
using EcotrackPlatform.API.Shared.Domain.Repositories;

namespace EcotrackPlatform.API.Organizations.Aplication.Internal.CommandServices;

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
            command.Status
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

    public async Task<Organization?> UpdateAsync(int id, string? name, string? description, string? status, List<int>? memberIds)
    {
        var organization = await repository.FindByIdWithMembersAsync(id);
        if (organization is null) return null;

        if (!string.IsNullOrWhiteSpace(name) || !string.IsNullOrWhiteSpace(description) || !string.IsNullOrWhiteSpace(status))
        {
            organization.Update(
                name ?? organization.Name,
                description ?? organization.Description,
                status ?? organization.Status);
        }

        if (memberIds is not null)
        {
            await ValidateProfilesExistAsync(memberIds);
            organization.SyncMembers(memberIds);
        }

        repository.Update(organization);
        await unitOfWork.CompleteAsync();
        return organization;
    }

    public async Task<bool> Handle(int id)
    {
        var organization = await repository.FindByIdAsync(id);
        if (organization is null) return false;

        var pendingInvitations = await invitationRepository.FindPendingByOrganizationAsync(id);
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