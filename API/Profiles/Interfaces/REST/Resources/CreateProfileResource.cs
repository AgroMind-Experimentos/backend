using EcotrackPlatform.API.Profiles.Domain.Model.ValueObjects;

namespace EcotrackPlatform.API.Profiles.Interfaces.REST.Resources
{
    public record CreateProfileResource(string Email, string DisplayName, string Password, UserRole Role);
}