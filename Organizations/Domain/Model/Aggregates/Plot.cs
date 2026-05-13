namespace EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;

public class Plot
{
    public int Id { get; }
    public string Name { get; private set; }
    public string Location { get; private set; }
    public double Area { get; private set; }
    public string Crop { get; private set; }
    public int OrganizationId { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    protected Plot() { }

    public Plot(string name, string location, double area, string crop, int organizationId)
    {
        ValidateString(name, nameof(Name));
        ValidateString(location, nameof(Location));
        ValidateArea(area);
        ValidateString(crop, nameof(Crop));

        Name = name;
        Location = location;
        Area = area;
        Crop = crop;
        OrganizationId = organizationId;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string? name, string? location, double? area, string? crop)
    {
        if (name is not null)
        {
            ValidateString(name, nameof(Name));
            Name = name;
        }

        if (location is not null)
        {
            ValidateString(location, nameof(Location));
            Location = location;
        }

        if (area is not null)
        {
            ValidateArea(area.Value);
            Area = area.Value;
        }

        if (crop is null) return;
        ValidateString(crop, nameof(Crop));
        Crop = crop;
    }

    private static void ValidateString(string value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{propertyName} cannot be empty or whitespace.");
        }
    }

    private static void ValidateArea(double area)
    {
        if (area <= 0)
        {
            throw new ArgumentException("Area must be greater than zero.");
        }
    }
}