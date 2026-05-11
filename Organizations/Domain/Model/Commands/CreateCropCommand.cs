namespace EcotrackPlatform.API.Organizations.Domain.Model.Commands;

public record CreateCropCommand(
    string Name,
    string Location,
    double Area,
    string Cultivation,
    int OrganizationId,
    List<int>? MemberIds = null
);