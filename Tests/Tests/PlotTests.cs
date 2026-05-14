using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using FluentAssertions;
using NUnit.Framework;

namespace EcotrackPlatform.Tests;

[TestFixture]
public class PlotTests
{
    [Test]
    public void CreatePlot_ShouldInitializeCorrectly()
    {
        // Arrange
        var name = "Parcela Norte";
        var location = "Huánuco, Perú";
        var area = 50.5;
        var crop = "Café";
        var orgId = 1;

        // Act
        var plot = new Plot(name, location, area, crop, orgId);

        // Assert
        plot.Name.Should().Be(name);
        plot.Area.Should().Be(area);
    }
    
    [Test]
    public void Update_WithValidData_ShouldUpdateCorrectly()
    {
        // Arrange
        var plot = new Plot("Lote Original", "Ubicación A", 100.0, "Maíz", 1);
        var newName = "Lote Renovado";
        var newArea = 150.5;

        // Act
        plot.Update(newName, "Ubicación B", newArea, "Soya");

        // Assert
        plot.Name.Should().Be(newName);
        plot.Area.Should().Be(newArea);
        plot.Location.Should().Be("Ubicación B");
        plot.Crop.Should().Be("Soya");
    }

    [Test]
    public void Update_WithInvalidArea_ShouldThrowArgumentException()
    {
        // Arrange
        var plot = new Plot("Terreno María", "Chanchamayo", 10.0, "Cacao", 1);
        double invalidArea = -5.0;

        // Act
        Action act = () => plot.Update(null, null, invalidArea, null);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Area must be greater than zero.");
        plot.Area.Should().Be(10.0);
    }
}