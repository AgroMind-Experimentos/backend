using EcotrackPlatform.API.Profiles.Domain.Model.Commands;
using EcotrackPlatform.API.Profiles.Interfaces.REST.Resources;

namespace EcotrackPlatform.API.Profiles.Interfaces.REST.Transform
{
    public static class CreateProfileCommandFromResourceAssembler
    {
        public static CreateProfileCommand ToCommand(CreateProfileResource r)
            => new(r.Email, r.DisplayName, r.Password, r.Role);
    }
}