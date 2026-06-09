namespace EcotrackPlatform.API.Organizations.Domain.Model.Commands;

public record CreateOrganizationCommand(
    string Name,
    string Description,
    double Latitude,
    double Longitude,
    int AgronomistId
);

