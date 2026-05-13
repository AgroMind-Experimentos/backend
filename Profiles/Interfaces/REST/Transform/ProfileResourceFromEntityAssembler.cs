using EcotrackPlatform.API.Profiles.Interfaces.REST.Resources;
using EcotrackPlatform.API.Profiles.Domain.Model.Aggregates;

namespace EcotrackPlatform.API.Profiles.Interfaces.REST.Transform
{
    public static class ProfileResourceFromEntityAssembler
    {
        public static ProfileResource ToResource(Profile e)
            => new(e.Id, e.Email, e.DisplayName, e.Role.ToString());
    }
}