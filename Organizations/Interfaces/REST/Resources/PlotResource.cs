namespace EcotrackPlatform.API.Organizations.Interfaces.REST.Resources;

public class PlotResource
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public double Area { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Cultivation { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}