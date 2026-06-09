namespace EcotrackPlatform.API.Organizations.Interfaces.REST.Resources;

public class OrganizationResource
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Latitude { get; set; } = double.NaN ;
    public double Longitude { get; set; } = double.NaN ;
    public int AgronomistId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<int> MemberIds { get; set; } = new();
}