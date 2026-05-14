namespace EcotrackPlatform.API.Profiles.Interfaces.REST.Resources
{
    public record SettingsResource(bool NotificationsEmail, string Locale, string Theme);
}