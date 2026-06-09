using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Organizations.Interfaces.REST.Resources;

namespace EcotrackPlatform.API.Organizations.Interfaces.REST.Transform;

public static class PlotResourceFromEntityAssembler
{
    public static PlotResource ToResource(Plot entity)
        => new PlotResource
        {
            Id = entity.Id,
            OrganizationId = entity.OrganizationId,
            Area = entity.Area,
            Name = entity.Name,
            Latitude = entity.Coordinates.Latitude,
            Longitude = entity.Coordinates.Longitude,
            Cultivation = entity.Crop,
            CreatedAt = entity.CreatedAt
        };
}