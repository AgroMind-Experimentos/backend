namespace EcotrackPlatform.API.Organizations.Domain.Model.Commands;

public record UpdatePlotCommand(
    int Id,
    string? Name,
    string? Location,
    double? Area,
    string? Crop
);