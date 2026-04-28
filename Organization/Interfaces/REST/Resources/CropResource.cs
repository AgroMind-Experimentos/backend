namespace EcotrackPlatform.API.Organization.Interfaces.REST.Resources;

public class CropResource
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public string Location { get; set; }
    public double Area { get; set; }
    public string Cultivation { get; set; }
    public DateTime CreatedAt { get; set; }
}