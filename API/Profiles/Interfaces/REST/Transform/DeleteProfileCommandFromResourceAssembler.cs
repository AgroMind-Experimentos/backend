namespace EcotrackPlatform.API.Profiles.Interfaces.REST.Transform;

using EcotrackPlatform.API.Profiles.Domain.Model.Commands;

public static class DeleteProfileCommandFromResourceAssembler
{
    public static DeleteProfileCommand ToCommand(int id)
        => new(id);
}