namespace EcotrackPlatform.API.Iam.Interfaces.REST.Resources;

public record ChangePasswordResource(string CurrentPassword, string NewPassword);