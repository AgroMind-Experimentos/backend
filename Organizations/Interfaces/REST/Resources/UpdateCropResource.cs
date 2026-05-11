namespace EcotrackPlatform.API.Organizations.Interfaces.REST.Resources;

public class UpdateCropResource
{
    public string? Name { get; set; }
    public string? Location { get; set; }
    public double? Area { get; set; }
    public string? Cultivation { get; set; }
    public List<int>? MemberIds { get; set; }
}