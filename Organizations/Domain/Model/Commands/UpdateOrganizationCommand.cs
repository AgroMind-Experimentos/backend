namespace EcotrackPlatform.API.Organizations.Domain.Model.Commands;

public record UpdateOrganizationCommand(
    int Id,
    string? Name,
    string? Description,
    string? Location,
    List<int>? MemberIds
);