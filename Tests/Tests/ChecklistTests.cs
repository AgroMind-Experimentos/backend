using EcotrackPlatform.API.Monitoringandcontrol.Domain.Model.Aggregates;
using EcotrackPlatform.API.Monitoringandcontrol.Domain.Model.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace EcotrackPlatform.Tests;

[TestFixture]
public class ChecklistTests
{
    [Test]
    public void AddChecklistItems_WhenValidData_ShouldListAllItems()
    {
        // Arrange
        var task = new TaskAggregate("Fertilización", "Aplicar NPK", 1, 10, 5);
        var checklist = new Checklist(task.Id, "Pasos de Fertilización");

        // Act
        checklist.AddItem("Preparar mezcla");
        checklist.AddItem("Verificar humedad del suelo");

        // Assert
        checklist.Items.Should().HaveCount(2);
        checklist.Items.Should().Contain(i => i.Description == "Preparar mezcla");
        checklist.Items[0].IsCompleted.Should().BeFalse();
    }

    [Test]
    public void ClearChecklist_WhenCalled_ShouldRemoveAllItemsFromTask()
    {
        // Arrange
        var checklist = new Checklist(1, "Lista a limpiar");
        checklist.AddItem("Item 1");
        checklist.AddItem("Item 2");
        checklist.Items.Should().HaveCount(2);

        // Act
        checklist.ClearItems();

        // Assert
        checklist.Items.Should().BeEmpty();
    }

    [Test]
    public void CompleteChecklistItem_WhenSetToTrue_ShouldReflectCompletedStatus()
    {
        // Arrange
        var item = new ChecklistItem("Revisar sensores de pH");
        item.IsCompleted.Should().BeFalse();

        // Act
        item.SetCompleted(true);

        // Assert
        item.IsCompleted.Should().BeTrue();
    }
}