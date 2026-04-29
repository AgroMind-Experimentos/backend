namespace EcotrackPlatform.API.Organization.Interfaces.REST.Resources;

public class UpdateOrganizationResource
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public List<int>? MemberIds { get; set; }
}