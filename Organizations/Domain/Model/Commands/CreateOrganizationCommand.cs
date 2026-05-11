namespace EcotrackPlatform.API.Organizations.Domain.Model.Commands;

public record CreateOrganizationCommand(
    string Name,
    string Description,
    string Status,
    int? AgronomistId = null
);

