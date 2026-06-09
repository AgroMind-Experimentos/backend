namespace EcotrackPlatform.API.Organizations.Interfaces.REST.Resources;

public class CreatePlotResource
{
    public string Name { get; set; } = string.Empty;
    public double Latitude { get; set; } = double.NaN;
    public double Longitude { get; set; } = double.NaN;
    public double Area { get; set; }
    public string Cultivation { get; set; } = string.Empty;
    public int OrganizationId { get; set; }
}