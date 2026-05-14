namespace EcotrackPlatform.API.Organizations.Interfaces.REST.Resources;

public class CreatePlotResource
{
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public double Area { get; set; }
    public string Cultivation { get; set; } = string.Empty;
    public int OrganizationId { get; set; }
}