namespace EcotrackPlatform.API.Organizations.Domain.Model.Commands;

public record UpdateOrganizationCommand(
    int Id,
    string? Name,
    string? Description,
    double? Latitude,
    double? Longitude,
    List<int>? MemberIds
);