using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Organizations.Domain.Model.ValueObjects;

namespace EcotrackPlatform.Tests.Organizations.Domain.Model;

[TestFixture]
public class PlotTests
{
    private const string ValidName = "Lote Norte";
    private const double ValidLatitude = -11.064932;
    private const double ValidLongitude = -75.340075;
    private const double ValidArea = 15.5;
    private const string ValidCrop = "Maíz";
    private const int OrganizationId = 1;

    [Test]
    public void Constructor_ValidArguments_ShouldCreatePlot()
    {
        // Act
        var coords = new Coordinates(ValidLatitude, ValidLongitude);
        var plot = new Plot(ValidName, coords, ValidArea, ValidCrop, OrganizationId);

        // Assert
        Assert.That(plot, Is.Not.Null);
        Assert.That(plot.Name, Is.EqualTo(ValidName));
        Assert.That(plot.Coordinates.Latitude, Is.EqualTo(ValidLatitude));
        Assert.That(plot.Coordinates.Longitude, Is.EqualTo(ValidLongitude));
        Assert.That(plot.Area, Is.EqualTo(ValidArea));
        Assert.That(plot.Crop, Is.EqualTo(ValidCrop));
        Assert.That(plot.OrganizationId, Is.EqualTo(OrganizationId));
        Assert.That(plot.CreatedAt, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(1)));
    }
}
