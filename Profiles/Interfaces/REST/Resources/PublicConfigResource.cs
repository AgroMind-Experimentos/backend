namespace EcotrackPlatform.API.Profiles.Interfaces.REST.Resources
{
    public record PublicConfigResource(bool RegistrationEnabled, string[] Locales, string[] Themes);
}