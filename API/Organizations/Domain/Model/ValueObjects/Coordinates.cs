namespace EcotrackPlatform.API.Organizations.Domain.Model.ValueObjects;

public record Coordinates
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }

    public Coordinates(double latitude, double longitude)
    {
        if (latitude is < -90.0 or > 90.0)
            throw new ArgumentOutOfRangeException(nameof(latitude), "The latitude must be between -90 and 90 degrees.");

        if (longitude is < -180.0 or > 180.0)
            throw new ArgumentOutOfRangeException(nameof(longitude), "The longitude must be between -180 and 180 degrees.");

        Latitude = latitude;
        Longitude = longitude;
    }
}