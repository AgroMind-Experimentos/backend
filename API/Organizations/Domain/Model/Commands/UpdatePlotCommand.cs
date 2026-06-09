namespace EcotrackPlatform.API.Organizations.Domain.Model.Commands;

public record UpdatePlotCommand(
    int Id,
    string? Name,
    double? Latitude,
    double? Longitude,
    double? Area,
    string? Crop
);