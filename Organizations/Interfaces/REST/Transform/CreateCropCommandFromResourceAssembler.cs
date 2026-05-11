using EcotrackPlatform.API.Organizations.Domain.Model.Commands;
using EcotrackPlatform.API.Organizations.Interfaces.REST.Resources;

namespace EcotrackPlatform.API.Organizations.Interfaces.REST.Transform;

public static class CreateCropCommandFromResourceAssembler
{
    public static CreateCropCommand ToCommand(CreateCropResource resource) =>
    new(resource.Name, resource.Location, resource.Area, resource.Cultivation, resource.OrganizationId, resource.MemberIds);
}