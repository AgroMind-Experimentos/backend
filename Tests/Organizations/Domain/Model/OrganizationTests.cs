using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Organizations.Domain.Model.ValueObjects;

namespace EcotrackPlatform.Tests.Organizations.Domain.Model;

[TestFixture]
public class OrganizationTests
{
    private const string ValidName = "Finca El Sol";
    private const string ValidDescription = "Producción agrícola orgánica";
    private const double ValidLatitude = -11.064932;
    private const double ValidLongitude = -75.340075;
    private const int AgronomistOwnerId = 42;

    [Test]
    public void Constructor_ValidArguments_ShouldCreateOrganization()
    {
        // Act
        var coords = new Coordinates(ValidLatitude, ValidLongitude);
        var org = new Organization(ValidName, ValidDescription, coords, AgronomistOwnerId);

        // Assert
        Assert.That(org, Is.Not.Null);
        Assert.That(org.Name, Is.EqualTo(ValidName));
        Assert.That(org.Description, Is.EqualTo(ValidDescription));
        Assert.That(org.Coordinates, Is.EqualTo(coords));
        Assert.That(org.AgronomistOwnerId, Is.EqualTo(AgronomistOwnerId));
        Assert.That(org.CreatedAt, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(1)));
        Assert.That(org.Members, Is.Empty);
        Assert.That(org.Plots, Is.Empty);
    }
    

    [Test]
    public void Update_ValidArguments_ShouldUpdateProperties()
    {
        // Arrange
        var coords = new Coordinates(ValidLatitude, ValidLongitude);
        var org = new Organization(ValidName, ValidDescription, coords, AgronomistOwnerId);
        var newName = "Hacienda La Luna";
        var newDescription = "Producción de cereales";
        var newCoords = new Coordinates(-18.064932, -72.340075);

        // Act
        org.Update(newName, newDescription, newCoords);

        // Assert
        Assert.That(org.Name, Is.EqualTo(newName));
        Assert.That(org.Description, Is.EqualTo(newDescription));
        Assert.That(org.Coordinates.Latitude, Is.EqualTo(newCoords.Latitude));
        Assert.That(org.Coordinates.Longitude, Is.EqualTo(newCoords.Longitude));
    }
}
