using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;

namespace EcotrackPlatform.Tests.Organizations.Domain.Model;

[TestFixture]
public class PlotTests
{
    private const string ValidName = "Lote Norte";
    private const string ValidLocation = "Coordenadas X:Y";
    private const double ValidArea = 15.5;
    private const string ValidCrop = "Maíz";
    private const int OrganizationId = 1;

    [Test]
    public void Constructor_ValidArguments_ShouldCreatePlot()
    {
        // Act
        var plot = new Plot(ValidName, ValidLocation, ValidArea, ValidCrop, OrganizationId);

        // Assert
        Assert.That(plot, Is.Not.Null);
        Assert.That(plot.Name, Is.EqualTo(ValidName));
        Assert.That(plot.Location, Is.EqualTo(ValidLocation));
        Assert.That(plot.Area, Is.EqualTo(ValidArea));
        Assert.That(plot.Crop, Is.EqualTo(ValidCrop));
        Assert.That(plot.OrganizationId, Is.EqualTo(OrganizationId));
        Assert.That(plot.CreatedAt, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(1)));
    }

    [TestCase("")]
    [TestCase(" ")]
    public void Constructor_InvalidName_ShouldThrowArgumentException(string invalidName)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            new Plot(invalidName, ValidLocation, ValidArea, ValidCrop, OrganizationId));
        Assert.That(ex.Message, Does.Contain("Name cannot be empty or whitespace"));
    }

    [TestCase(0)]
    [TestCase(-5.5)]
    public void Constructor_InvalidArea_ShouldThrowArgumentException(double invalidArea)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            new Plot(ValidName, ValidLocation, invalidArea, ValidCrop, OrganizationId));
        Assert.That(ex.Message, Does.Contain("Area must be greater than zero"));
    }

    [Test]
    public void Update_ValidArguments_ShouldUpdateProperties()
    {
        // Arrange
        var plot = new Plot(ValidName, ValidLocation, ValidArea, ValidCrop, OrganizationId);
        var newName = "Lote Sur";
        var newLocation = "Coordenadas Z:W";
        var newArea = 20.0;
        var newCrop = "Trigo";

        // Act
        plot.Update(newName, newLocation, newArea, newCrop);

        // Assert
        Assert.That(plot.Name, Is.EqualTo(newName));
        Assert.That(plot.Location, Is.EqualTo(newLocation));
        Assert.That(plot.Area, Is.EqualTo(newArea));
        Assert.That(plot.Crop, Is.EqualTo(newCrop));
    }

    [Test]
    public void Update_NullArguments_ShouldNotUpdateProperties()
    {
        // Arrange
        var plot = new Plot(ValidName, ValidLocation, ValidArea, ValidCrop, OrganizationId);

        // Act
        plot.Update(null, null, null, null);

        // Assert
        Assert.That(plot.Name, Is.EqualTo(ValidName));
        Assert.That(plot.Location, Is.EqualTo(ValidLocation));
        Assert.That(plot.Area, Is.EqualTo(ValidArea));
        Assert.That(plot.Crop, Is.EqualTo(ValidCrop));
    }

    [TestCase("")]
    [TestCase(" ")]
    public void Update_InvalidLocation_ShouldThrowArgumentException(string invalidLocation)
    {
        // Arrange
        var plot = new Plot(ValidName, ValidLocation, ValidArea, ValidCrop, OrganizationId);

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            plot.Update(null, invalidLocation, null, null));
        Assert.That(ex.Message, Does.Contain("Location cannot be empty or whitespace"));
    }
}
