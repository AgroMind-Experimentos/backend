using EcotrackPlatform.API.Organizations.Application.Internal.CommandServices.Organizations;
using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Organizations.Domain.Model.Commands;
using EcotrackPlatform.API.Organizations.Domain.Model.ValueObjects;
using EcotrackPlatform.API.Organizations.Domain.Repositories;
using EcotrackPlatform.API.Profiles.Domain.Repositories;
using EcotrackPlatform.API.Shared.Domain.Repositories;

namespace EcotrackPlatform.API.Organizations.Application.Internal.CommandServices.Plots;

public enum UpdatePlotError
{
    None,
    PlotNotFound,
    OrganizationNotFound,
    ProfileNotFound,
    ProfileNotInOrganization,
    InvalidPlotData
}

public record UpdatePlotResult(Plot? Plot = null, UpdatePlotError Error = UpdatePlotError.None)
{
    public bool Success => Error == UpdatePlotError.None;
}

public class UpdatePlotCommandService(
    IPlotRepository plotRepository,
    IOrganizationRepository organizationRepository,
    IProfileRepository profileRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<UpdatePlotResult> UpdateAsync(UpdatePlotCommand command)
    {
        var plot = await plotRepository.FindByIdAsync(command.Id);
        if (plot is null) return new UpdatePlotResult(Error: UpdatePlotError.PlotNotFound);

        var organization = await organizationRepository.FindByIdWithMembersAsync(plot.OrganizationId);
        if (organization is null)
            return new UpdatePlotResult(Error: UpdatePlotError.OrganizationNotFound);

        try
        {
            Coordinates? coordinates = null;
        
            if (command.Latitude.HasValue && command.Longitude.HasValue)
            {
                coordinates = new Coordinates(command.Latitude.Value, command.Longitude.Value);
            }
            else if (command.Latitude.HasValue || command.Longitude.HasValue)
            {
                return new UpdatePlotResult(Error: UpdatePlotError.InvalidPlotData);
            }
            
            plot.Update(
                command.Name,
                coordinates,
                command.Area,
                command.Crop
            );

            plotRepository.Update(plot);
            await unitOfWork.CompleteAsync();

            return new UpdatePlotResult(Plot: plot);
        }
        catch (Exception ex)
        {
            return new UpdatePlotResult(Error: UpdatePlotError.InvalidPlotData);
        }
    }

    private async Task<UpdatePlotError> ValidateMembersAsync(Organization organization, IEnumerable<int> memberIds)
    {
        var organizationMemberIds = organization.Members.Select(member => member.ProfileId).ToHashSet();

        foreach (var memberId in memberIds.Distinct())
        {
            var profile = await profileRepository.FindByIdAsync(memberId);
            if (profile is null) return UpdatePlotError.ProfileNotFound;

            if (!organizationMemberIds.Contains(memberId)) return UpdatePlotError.ProfileNotInOrganization;
        }

        return UpdatePlotError.None;
    }
}