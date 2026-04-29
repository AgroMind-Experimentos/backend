using EcotrackPlatform.API.Organization.Aplication.Services;
using EcotrackPlatform.API.Organization.Domain.Model.Commands;
using EcotrackPlatform.API.Organization.Domain.Repositories;
using EcotrackPlatform.API.Profile.Domain.Repositories;
using EcotrackPlatform.API.Shared.Domain.Repositories;

namespace EcotrackPlatform.API.Organization.Aplication.Internal.CommandServices;

public class OrganizationCommandService(
    IOrganizationRepository repository,
    IProfileRepository profileRepository,
    IUnitOfWork unitOfWork)
    : IOrganizationCommandService
{
    public async Task<Domain.Model.Aggregates.Organization> Handle(CreateOrganizationCommand command)
    {
        if (command.MemberIds is not null && command.MemberIds.Count > 0)
            await ValidateProfilesExistAsync(command.MemberIds);

        var org = new Domain.Model.Aggregates.Organization(
            command.Name,
            command.Description,
            command.Status
        );

        await repository.AddAsync(org);
        await unitOfWork.CompleteAsync();

        if (command.MemberIds is not null && command.MemberIds.Count > 0)
        {
            org.SyncMembers(command.MemberIds);
            repository.Update(org);
            await unitOfWork.CompleteAsync();
        }

        return org;
    }

    public async Task<Domain.Model.Aggregates.Organization?> UpdateAsync(int id, string? name, string? description, string? status, List<int>? memberIds)
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
        if (organization == null) return false;

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