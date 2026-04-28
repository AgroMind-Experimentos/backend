namespace EcotrackPlatform.API.Organization.Interfaces.REST.Resources;

public class CreateCropResource
{
    public string Name { get; set; }
    public string Location { get; set; }
    public double Area { get; set; }
    public string Cultivation { get; set; }
    public int OrganizationId { get; set; }
}