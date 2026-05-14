using EcotrackPlatform.API.Profiles.Domain.Model.ValueObjects;

namespace EcotrackPlatform.API.Profiles.Domain.Model.Commands
{
    public record CreateProfileCommand(string Email, string DisplayName, string Password, UserRole Role);
}