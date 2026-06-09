using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Organizations.Domain.Model.Commands;
using EcotrackPlatform.API.Organizations.Domain.Model.ValueObjects;
using EcotrackPlatform.API.Organizations.Domain.Repositories;
using EcotrackPlatform.API.Profiles.Domain.Repositories;
using EcotrackPlatform.API.Shared.Domain.Repositories;

namespace EcotrackPlatform.API.Organizations.Application.Internal.CommandServices.Organizations;

public enum UpdateOrganizationError
{
    None,
    OrganizationNotFound,
    ProfileNotFound,
    InvalidOrganizationData
}

public record UpdateOrganizationResult(Organization? Organization = null, UpdateOrganizationError Error = UpdateOrganizationError.None)
{
    public bool Success => Error == UpdateOrganizationError.None;
}

public class UpdateOrganizationCommandService(
    IOrganizationRepository repository,
    IProfileRepository profileRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<UpdateOrganizationResult> UpdateAsync(UpdateOrganizationCommand command)
    {
        var organization = await repository.FindByIdWithMembersAsync(command.Id);
        if (organization is null) return new UpdateOrganizationResult(Error: UpdateOrganizationError.OrganizationNotFound);

        try
        {
            Coordinates? coordinates = null;
        
            if (command.Latitude.HasValue && command.Longitude.HasValue)
            {
                coordinates = new Coordinates(command.Latitude.Value, command.Longitude.Value);
            }
            else if (command.Latitude.HasValue || command.Longitude.HasValue)
            {
                return new UpdateOrganizationResult(Error: UpdateOrganizationError.InvalidOrganizationData);
            }
            
            organization.Update(
                command.Name,
                command.Description,
                coordinates
            );

            if (command.MemberIds is not null)
            {
                var profileError = await ValidateProfilesExistAsync(command.MemberIds);
                if (profileError != UpdateOrganizationError.None)
                    return new UpdateOrganizationResult(Error: profileError);

                organization.SyncMembers(command.MemberIds);
            }

            repository.Update(organization);
            await unitOfWork.CompleteAsync();
            return new UpdateOrganizationResult(Organization: organization);
        }
        catch (Exception)
        {
            return new UpdateOrganizationResult(Error: UpdateOrganizationError.InvalidOrganizationData);
        }
    }

    private async Task<UpdateOrganizationError> ValidateProfilesExistAsync(IEnumerable<int> profileIds)
    {
        foreach (var profileId in profileIds.Distinct())
        {
            var profile = await profileRepository.FindByIdAsync(profileId);
            if (profile is null) return UpdateOrganizationError.ProfileNotFound;
        }
        return UpdateOrganizationError.None;
    }
}