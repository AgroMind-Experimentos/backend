namespace EcotrackPlatform.API.Organizations.Interfaces.REST.Resources;

public class UpdateOrganizationResource
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public List<int>? MemberIds { get; set; }
}