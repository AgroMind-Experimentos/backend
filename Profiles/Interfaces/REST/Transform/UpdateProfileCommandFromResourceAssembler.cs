using EcotrackPlatform.API.Profiles.Domain.Model.Commands;
using EcotrackPlatform.API.Profiles.Interfaces.REST.Resources;

namespace EcotrackPlatform.API.Profiles.Interfaces.REST.Transform
{
    public static class UpdateProfileCommandFromResourceAssembler
    {
        public static UpdateProfileCommand ToCommand(int id, UpdateProfileResource r)
            => new(id, r.DisplayName, r.Email);
    }
}