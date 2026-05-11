using EcotrackPlatform.API.Organizations.Aplication.Services;
using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Organizations.Domain.Model.Commands;
using EcotrackPlatform.API.Organizations.Domain.Repositories;
using EcotrackPlatform.API.Profiles.Domain.Repositories;
using EcotrackPlatform.API.Shared.Domain.Repositories;

namespace EcotrackPlatform.API.Organizations.Aplication.Internal.CommandServices;

public class PlotCommandService(
    IPlotRepository plotRepository,
    IOrganizationRepository organizationRepository,
    IProfileRepository profileRepository,
    IUnitOfWork unitOfWork) : IPlotCommandService
{
    public async Task<Plot> Handle(CreatePlotCommand command)
    {
        var organization = await organizationRepository.FindByIdWithMembersAsync(command.OrganizationId);
        if (organization is null)
        {
            throw new InvalidOperationException($"Organization with id {command.OrganizationId} does not exist.");
        }

        if (command.MemberIds is not null && command.MemberIds.Count > 0)
            await ValidateMembersBelongToOrganizationAsync(organization, command.MemberIds);

        Plot plot = new Plot(
            command.Name,
            command.Location,
            command.Area,
            command.Cultivation,
            command.OrganizationId);

        await plotRepository.AddAsync(plot);
        await unitOfWork.CompleteAsync();

        if (command.MemberIds is not null && command.MemberIds.Count > 0)
        {
            plot.SyncMembers(command.MemberIds);
            plotRepository.Update(plot);
            await unitOfWork.CompleteAsync();
        }

        return plot;
    }

    public async Task<Plot?> UpdateAsync(int id, string? name, string? location, double? area, string? cultivation, List<int>? memberIds)
    {
        var plot = await plotRepository.FindByIdWithMembersAsync(id);
        if (plot is null) return null;

        var organization = await organizationRepository.FindByIdWithMembersAsync(plot.OrganizationId);
        if (organization is null)
        {
            throw new InvalidOperationException($"Organization with id {plot.OrganizationId} does not exist.");
        }

        plot.Update(
            name ?? plot.Name,
            location ?? plot.Location,
            area ?? plot.Area,
            cultivation ?? plot.Cultivation);

        if (memberIds is not null)
        {
            await ValidateMembersBelongToOrganizationAsync(organization, memberIds);
            plot.SyncMembers(memberIds);
        }

        plotRepository.Update(plot);
        await unitOfWork.CompleteAsync();
        return plot;
    }

    public async Task<bool> Handle(int id)
    {
        var plot = await plotRepository.FindByIdAsync(id);
        if (plot == null) return false;

        plotRepository.Remove(plot);
        await unitOfWork.CompleteAsync();
        return true;
    }

    private async Task ValidateMembersBelongToOrganizationAsync(Organization organization, IEnumerable<int> memberIds)
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