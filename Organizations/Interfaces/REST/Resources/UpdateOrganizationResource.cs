namespace EcotrackPlatform.API.Organizations.Interfaces.REST.Resources;

public class UpdateOrganizationResource
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public List<int>? MemberIds { get; set; }
}