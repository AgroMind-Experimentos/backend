namespace EcotrackPlatform.API.Organization.Interfaces.REST.Resources;

public class CreateCropResource
{
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public double Area { get; set; }
    public string Cultivation { get; set; } = string.Empty;
    public int OrganizationId { get; set; }
    public List<int>? MemberIds { get; set; }
}