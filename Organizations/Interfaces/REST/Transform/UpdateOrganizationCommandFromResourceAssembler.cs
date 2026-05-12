using EcotrackPlatform.API.Organizations.Domain.Model.Commands;
using EcotrackPlatform.API.Organizations.Interfaces.REST.Resources;

namespace EcotrackPlatform.API.Organizations.Interfaces.REST.Transform;

public static class UpdateOrganizationCommandFromResourceAssembler
{
    public static UpdateOrganizationCommand ToCommand(int id, UpdateOrganizationResource resource)
        => new(
            id,
            resource.Name,
            resource.Description,
            resource.Location,
            resource.MemberIds
        );
}