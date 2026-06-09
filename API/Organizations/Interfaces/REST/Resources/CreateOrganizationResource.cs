namespace EcotrackPlatform.API.Organizations.Interfaces.REST.Resources;

public class CreateOrganizationResource
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Latitude { get; set; } = double.NaN ;
    public double Longitude { get; set; } = double.NaN ;
    public int AgronomistId { get; set; }
}