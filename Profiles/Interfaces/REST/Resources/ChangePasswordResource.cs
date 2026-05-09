namespace EcotrackPlatform.API.Profiles.Interfaces.REST.Resources;

public record ChangePasswordResource(string CurrentPassword, string NewPassword);