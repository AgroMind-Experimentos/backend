using EcotrackPlatform.API.Organizations.Domain.Model.Commands;
using EcotrackPlatform.API.Organizations.Interfaces.REST.Resources;

namespace EcotrackPlatform.API.Organizations.Interfaces.REST.Transform;

public static class CreateOrganizationCommandFromResourceAssembler
{
    public static CreateOrganizationCommand ToCommand(CreateOrganizationResource resource)
        => new(
            resource.Name,
            resource.Description,
            resource.Location,
            resource.AgronomistId
        );
}
