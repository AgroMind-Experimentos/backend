using EcotrackPlatform.API.Profile.Domain.Model.ValueObjects;

namespace EcotrackPlatform.API.Profile.Interfaces.REST.Resources
{
    public record CreateProfileResource(string Email, string DisplayName, string Password, UserRole Role);
}