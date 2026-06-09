using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Organizations.Domain.Model.ValueObjects;
using FluentAssertions;
using NUnit.Framework;

namespace EcotrackPlatform.API.Tests;

[TestFixture]
public class OrganizationTests
{
    [Test]
    public void AddPlots_WhenPlotsAreAdded_ShouldListAndMatchAllPlots()
    {
        // Arrange

        var coordinates = new Coordinates(-11.064960, -75.340056);
        var organization = new Organization("Finca La Esperanza", "Producción de café premium", coordinates, 5);
        
        var plot1 = new Plot("Lote Norte", "Zona Alta", 10.5, "Café Arabica", organization.Id);
        var plot2 = new Plot("Lote Sur", "Zona Baja", 15.0, "Café Robusta", organization.Id);
        
        var expectedPlots = new List<Plot> { plot1, plot2 };

        // Act
        organization.Plots.Add(plot1);
        organization.Plots.Add(plot2);

        // Assert
        organization.Plots.Should().NotBeEmpty();
        organization.Plots.Should().HaveCount(2);
        organization.Plots.Should().BeEquivalentTo(expectedPlots);
    }

    [Test]
    public void GetPlots_WhenNoPlotsAreAdded_ShouldReturnEmptyList()
    {
        // Arrange
        var coordinates = new Coordinates(-11.064960, -75.340056);
        var organization = new Organization("Cooperativa Agraria", "Distribución mayorista", coordinates, 3);

        // Act

        // Assert
        organization.Plots.Should().NotBeNull();
        organization.Plots.Should().BeEmpty();
        organization.Plots.Should().HaveCount(0);
    }
    
    [Test]
    public void RemovePlot_WhenPlotExistsInOrganization_ShouldRemoveFromListAndReturnEmpty()
    {
        // Arrange
        var coordinates = new Coordinates(-11.064960, -75.340056);
        var organization = new Organization("Finca San Juan", "Producción orgánica", coordinates, 3);
        var plot = new Plot("Lote Cafetal", "Ladera Norte", 12.5, "Café Caturra", organization.Id);
        
        organization.Plots.Add(plot);
        organization.Plots.Should().Contain(plot);

        // Act
        organization.Plots.Remove(plot);

        // Assert
        organization.Plots.Should().NotContain(plot);
        organization.Plots.Should().BeEmpty();
        organization.Plots.Should().HaveCount(0);
    }
    
    [Test]
    public void CreateOrganization_WithValidData_ShouldInitializeCorrectly()
    {
        // Arrange
        var name = "Cooperativa Agroecológica";
        var description = "Dedicada al cultivo de cacao orgánico";
        var coordinates = new Coordinates(-11.064960, -75.340056);

        // Act
        var organization = new Organization(name, description, coordinates, 1);

        // Assert
        organization.Name.Should().Be(name);
        organization.Description.Should().Be(description);
    }
    
    [Test]
    public void Update_WithValidData_ShouldUpdatePropertiesCorrectly()
    {
        // Arrange
        var coordinates = new Coordinates(-11.064960, -75.340056);
        var organization = new Organization("Nombre Original", "Descripción Original", coordinates, 1);
        var newName = "Nombre Actualizado";
        var newDescription = "Nueva Descripción";

        // Act
        organization.Update(newName, newDescription, null);

        // Assert
        organization.Name.Should().Be(newName);
        organization.Description.Should().Be(newDescription);
    }

    [Test]
    public void Update_WithEmptyName_ShouldThrowArgumentException()
    {
        // Arrange
        var coordinates = new Coordinates(-11.064960, -75.340056);
        var organization = new Organization("EcoFarm", "Desc", coordinates, 2);

        // Act
        var newCoordinates = new Coordinates(-16.064960, -78.340056);
        Action act = () => organization.Update("", "Nueva Desc", newCoordinates);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Name cannot be empty or whitespace.");
        organization.Name.Should().Be("EcoFarm");
    }
}