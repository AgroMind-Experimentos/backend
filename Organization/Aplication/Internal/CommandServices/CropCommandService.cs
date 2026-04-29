using EcotrackPlatform.API.Organization.Aplication.Services;
using EcotrackPlatform.API.Organization.Domain.Model.Commands;
using EcotrackPlatform.API.Organization.Domain.Model.Aggregates;
using EcotrackPlatform.API.Organization.Domain.Repositories;
using EcotrackPlatform.API.Profile.Domain.Repositories;
using EcotrackPlatform.API.Shared.Domain.Repositories;


namespace EcotrackPlatform.API.Organization.Aplication.Internal.CommandServices;

public class CropCommandService(
    ICropRepository cropRepository,
    IOrganizationRepository organizationRepository,
    IProfileRepository profileRepository,
    IUnitOfWork unitOfWork) : ICropCommandService
{
    public async Task<Crop> Handle(CreateCropCommand command)
    {
        var organization = await organizationRepository.FindByIdWithMembersAsync(command.OrganizationId);
        if (organization is null)
        {
            throw new InvalidOperationException($"Organization with id {command.OrganizationId} does not exist.");
        }

        if (command.MemberIds is not null && command.MemberIds.Count > 0)
            await ValidateMembersBelongToOrganizationAsync(organization, command.MemberIds);

        Crop crop = new Crop(
            command.Name,
            command.Location,
            command.Area,
            command.Cultivation,
            command.OrganizationId);

        await cropRepository.AddAsync(crop);
        await unitOfWork.CompleteAsync();

        if (command.MemberIds is not null && command.MemberIds.Count > 0)
        {
            crop.SyncMembers(command.MemberIds);
            cropRepository.Update(crop);
            await unitOfWork.CompleteAsync();
        }

        return crop;
    }

    public async Task<Crop?> UpdateAsync(int id, string? name, string? location, double? area, string? cultivation, List<int>? memberIds)
    {
        var crop = await cropRepository.FindByIdWithMembersAsync(id);
        if (crop is null) return null;

        var organization = await organizationRepository.FindByIdWithMembersAsync(crop.OrganizationId);
        if (organization is null)
        {
            throw new InvalidOperationException($"Organization with id {crop.OrganizationId} does not exist.");
        }

        crop.Update(
            name ?? crop.Name,
            location ?? crop.Location,
            area ?? crop.Area,
            cultivation ?? crop.Cultivation);

        if (memberIds is not null)
        {
            await ValidateMembersBelongToOrganizationAsync(organization, memberIds);
            crop.SyncMembers(memberIds);
        }

        cropRepository.Update(crop);
        await unitOfWork.CompleteAsync();
        return crop;
    }

    public async Task<bool> Handle(int id)
    {
        var crop = await cropRepository.FindByIdAsync(id);
        if (crop == null) return false;

        cropRepository.Remove(crop);
        await unitOfWork.CompleteAsync();
        return true;
    }

    private async Task ValidateMembersBelongToOrganizationAsync(Domain.Model.Aggregates.Organization organization, IEnumerable<int> memberIds)
    {
        var organizationMemberIds = organization.Members.Select(member => member.ProfileId).ToHashSet();

        foreach (var memberId in memberIds.Distinct())
        {
            var profile = await profileRepository.FindByIdAsync(memberId);
            if (profile is null)
            {
                throw new InvalidOperationException($"Profile with id {memberId} does not exist.");
            }

            if (!organizationMemberIds.Contains(memberId))
            {
                throw new InvalidOperationException($"Profile with id {memberId} does not belong to organization {organization.Id}.");
            }
        }
    }
}