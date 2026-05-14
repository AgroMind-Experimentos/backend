namespace EcotrackPlatform.API.Iam.Domain.Model.Commands;

public record ChangePasswordCommand(
    int Id,
    string CurrentPassword,
    string NewPassword
);