namespace EcotrackPlatform.API.Organizations.Interfaces.REST.Resources;

public class UpdatePlotResource
{
    public string? Name { get; set; }
    public string? Location { get; set; }
    public double? Area { get; set; }
    public string? Crop { get; set; }
}