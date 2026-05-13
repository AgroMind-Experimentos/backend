using EcotrackPlatform.API.Organizations.Domain.Model.Commands;
using EcotrackPlatform.API.Organizations.Interfaces.REST.Resources;

namespace EcotrackPlatform.API.Organizations.Interfaces.REST.Transform;

public static class UpdatePlotCommandFromResourceAssembler
{
    public static UpdatePlotCommand ToCommand(int id, UpdatePlotResource resource)
        => new(
            id,
            resource.Name,
            resource.Location,
            resource.Area,
            resource.Crop
        );
}