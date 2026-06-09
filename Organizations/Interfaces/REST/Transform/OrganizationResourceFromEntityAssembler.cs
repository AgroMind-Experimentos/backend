using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Organizations.Interfaces.REST.Resources;

namespace EcotrackPlatform.API.Organizations.Interfaces.REST.Transform;

public static class OrganizationResourceFromEntityAssembler
{
    public static OrganizationResource ToResource(Organization entity)
        => new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Latitude = entity.Coordinates.Latitude,
            Longitude = entity.Coordinates.Longitude,
            AgronomistId = entity.AgronomistOwnerId,
            CreatedAt = entity.CreatedAt,
            MemberIds = entity.Members.Select(member => member.ProfileId).ToList()
        };
}