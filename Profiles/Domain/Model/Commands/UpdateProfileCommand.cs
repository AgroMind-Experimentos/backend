namespace EcotrackPlatform.API.Profiles.Domain.Model.Commands
{
    // Campos opcionales para parches parciales
    public record UpdateProfileCommand(int Id, string? DisplayName, string? Email);
}