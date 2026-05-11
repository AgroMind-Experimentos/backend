using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Organizations.Interfaces.REST.Resources;

namespace EcotrackPlatform.API.Organizations.Interfaces.REST.Transform;

public static class CropResourceFromEntityAssembler
{
    public static CropResource ToResource(Crop entity)
        => new CropResource
        {
            Id = entity.Id,
            Name = entity.Name,
            Location = entity.Location,
            Area = entity.Area,
            Cultivation = entity.Cultivation,
            CreatedAt = entity.CreatedAt,
            MemberIds = entity.Members.Select(member => member.ProfileId).ToList()
        };
}