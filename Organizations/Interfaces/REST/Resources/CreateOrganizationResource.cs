namespace EcotrackPlatform.API.Organizations.Interfaces.REST.Resources;

public class CreateOrganizationResource
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? AgronomistId { get; set; }
}