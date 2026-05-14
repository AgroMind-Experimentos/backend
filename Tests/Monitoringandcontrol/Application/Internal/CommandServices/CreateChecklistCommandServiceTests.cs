using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EcotrackPlatform.API.Monitoringandcontrol.Application.Internal.CommandServices;
using EcotrackPlatform.API.Monitoringandcontrol.Domain.Model.Entities;
using EcotrackPlatform.API.Monitoringandcontrol.Domain.Repositories;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace EcotrackPlatform.Tests.Monitoringandcontrol.Application.Internal.CommandServices;

[TestFixture]
public class CreateChecklistCommandServiceTests
{
    private Mock<IChecklistRepository> _checklistRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _checklistRepositoryMock = new Mock<IChecklistRepository>();
    }

    [Test]
    public async Task Handle_ValidData_ShouldCreateChecklistWithItemsAndReturnId()
    {
        var service = new CreateChecklistCommandService(_checklistRepositoryMock.Object);
        var taskId = 10;
        var title = "Verificación de Herramientas";
        var items = new List<string> { "Pala", "Tractor", "Semillas" };
        var expectedId = 5;

        _checklistRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Checklist>()))
            .Callback<Checklist>(c => 
            {
                // Verify the content inside the AddAsync method call
                c.TaskId.Should().Be(taskId);
                c.Title.Should().Be(title);
                c.Items.Should().HaveCount(3);
                c.Items.Select(i => i.Description).Should().BeEquivalentTo(items);
            })
            .ReturnsAsync(expectedId);

        var result = await service.Handle(taskId, title, items);

        result.Should().Be(expectedId);
        _checklistRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Checklist>()), Times.Once);
    }
}
