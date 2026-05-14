using EcotrackPlatform.API.Iam.Domain.Model.Commands;
using EcotrackPlatform.API.Iam.Interfaces.REST.Resources;

namespace EcotrackPlatform.API.Iam.Interfaces.REST.Transform;

public static class ChangePasswordCommandFromResourceAssembler
{
    public static ChangePasswordCommand ToCommand(int id, ChangePasswordResource resource)
        => new(
            id,
            resource.CurrentPassword,
            resource.NewPassword
        );
}