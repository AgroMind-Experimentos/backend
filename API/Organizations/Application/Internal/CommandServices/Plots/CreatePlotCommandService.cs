using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Organizations.Domain.Model.Commands;
using EcotrackPlatform.API.Organizations.Domain.Model.ValueObjects;
using EcotrackPlatform.API.Organizations.Domain.Repositories;
using EcotrackPlatform.API.Shared.Domain.Repositories;

namespace EcotrackPlatform.API.Organizations.Application.Internal.CommandServices.Plots;

public enum CreatePlotError
{
    None,
    OrganizationNotFound,
    InvalidPlotData
}

public record CreatePlotResult(Plot? Plot = null, CreatePlotError Error = CreatePlotError.None)
{
    public bool Success => Error == CreatePlotError.None;
}

public class CreatePlotCommandService(
    IPlotRepository plotRepository,
    IOrganizationRepository organizationRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<CreatePlotResult> CreateAsync(CreatePlotCommand command)
    {
        var organization = await organizationRepository.FindByIdWithMembersAsync(command.OrganizationId);
        if (organization is null)
            return new CreatePlotResult(Error: CreatePlotError.OrganizationNotFound);

        try
        {
            var coordinates = new Coordinates(command.Latitude, command.Longitude);
            var plot = new Plot(
                command.Name,
                coordinates,
                command.Area,
                command.Crop,
                command.OrganizationId
            );

            await plotRepository.AddAsync(plot);
            await unitOfWork.CompleteAsync();

            return new CreatePlotResult(Plot: plot);
        }
        catch (Exception ex)
        {
            return new CreatePlotResult(Error: CreatePlotError.InvalidPlotData);
        }
    }
}