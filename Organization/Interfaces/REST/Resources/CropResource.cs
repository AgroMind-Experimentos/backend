namespace EcotrackPlatform.API.Organization.Interfaces.REST.Resources;

public class CropResource
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public string Location { get; set; } = string.Empty;
    public double Area { get; set; }
    public string Cultivation { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<int> MemberIds { get; set; } = new();
}