namespace EcotrackPlatform.API.Organizations.Domain.Model.Commands;

public record CreatePlotCommand(
    string Name,
    double Latitude,
    double Longitude,
    double Area,
    string Crop,
    int OrganizationId
);