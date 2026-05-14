using System.Text;
using EcotrackPlatform.API.Report.Application.Internal.CommandServices;
using EcotrackPlatform.API.Report.Domain.Commands;
using EcotrackPlatform.API.Report.Domain.Model.ValueObjects;
using EcotrackPlatform.API.Report.Domain.Repositories;
using EcotrackPlatform.API.Report.Domain.Services;
using EcotrackPlatform.API.Shared.Domain.Repositories;

namespace EcotrackPlatform.Tests.Report.Application.Internal.CommandServices;

[TestFixture]
public class ReportCommandServiceTests
{
    private Mock<IReportRepository> _reportRepositoryMock;
    private Mock<IUnitOfWork> _uowMock;
    private Mock<ITaskReportGeneratorService> _taskReportGeneratorMock;

    [SetUp]
    public void Setup()
    {
        _reportRepositoryMock = new Mock<IReportRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _taskReportGeneratorMock = new Mock<ITaskReportGeneratorService>();
    }

    [Test]
    public async Task HandleRequestTaskReportCommand_ValidCommand_ShouldCreateReportAndReturn()
    {
        var service = new ReportCommandService(_reportRepositoryMock.Object, _uowMock.Object, _taskReportGeneratorMock.Object);
        var command = new RequestTaskReportCommand(
            PlotId: new PlotId(1),
            RequestedBy: new ProfileId(5),
            PeriodStart: DateTime.UtcNow.AddDays(-10), 
            PeriodEnd: DateTime.UtcNow
        );

        var result = await service.HandleRequestTaskReportCommand(command);

        result.Should().NotBeNull();
        result.Status.Should().Be(ReportStatus.Requested);
        result.PlotId.Value.Should().Be(1);
        result.RequestedBy.Value.Should().Be(5);
        result.Type.Should().Be(ReportType.ActivitySummary);

        _reportRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<API.Report.Domain.Model.Report>()), Times.Once);
        _uowMock.Verify(uow => uow.CompleteAsync(), Times.Once);
    }

    [Test]
    public void HandleRequestTaskReportCommand_MissingPlotId_ShouldThrowArgumentException()
    {
        var service = new ReportCommandService(_reportRepositoryMock.Object, _uowMock.Object, _taskReportGeneratorMock.Object);
        var command = new RequestTaskReportCommand(
            PlotId: null, 
            RequestedBy: new ProfileId(5),
            PeriodStart: DateTime.UtcNow.AddDays(-10), 
            PeriodEnd: DateTime.UtcNow);

        Assert.ThrowsAsync<ArgumentException>(async () => await service.HandleRequestTaskReportCommand(command));

        _reportRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<API.Report.Domain.Model.Report>()), Times.Never);
        _uowMock.Verify(uow => uow.CompleteAsync(), Times.Never);
    }

    [Test]
    public async Task HandleGenerateReportCommand_ValidReport_ShouldGenerateContentAndComplete()
    {
        var service = new ReportCommandService(_reportRepositoryMock.Object, _uowMock.Object, _taskReportGeneratorMock.Object);
        var reportId = 1;
        var command = new GenerateReportCommand(reportId);

        var report = API.Report.Domain.Model.Report.Request(ReportType.ActivitySummary, new PlotId(1), new ProfileId(5), DateTime.UtcNow.AddDays(-10), DateTime.UtcNow);
        var expectedJson = "{ \"data\": \"test\" }";

        _reportRepositoryMock.Setup(repo => repo.FindByIdAsync(reportId)).ReturnsAsync(report);
        _taskReportGeneratorMock.Setup(svc => svc.GenerateReportJsonAsync(report)).ReturnsAsync(expectedJson);

        var result = await service.HandleGenerateReportCommand(command);

        result.Should().NotBeNull();
        result.Status.Should().Be(ReportStatus.Generated);
        result.Content.Should().BeEquivalentTo(Encoding.UTF8.GetBytes(expectedJson));

        _uowMock.Verify(uow => uow.CompleteAsync(), Times.Exactly(2));
    }

    [Test]
    public async Task HandleGenerateReportCommand_GeneratorThrows_ShouldMarkFailedAndThrow()
    {
        var service = new ReportCommandService(_reportRepositoryMock.Object, _uowMock.Object, _taskReportGeneratorMock.Object);
        var reportId = 1;
        var command = new GenerateReportCommand(reportId);

        var report = API.Report.Domain.Model.Report.Request(ReportType.ActivitySummary, new PlotId(), new ProfileId(5), DateTime.UtcNow.AddDays(-10), DateTime.UtcNow);

        _reportRepositoryMock.Setup(repo => repo.FindByIdAsync(reportId)).ReturnsAsync(report);
        _taskReportGeneratorMock.Setup(svc => svc.GenerateReportJsonAsync(report))
            .ThrowsAsync(new Exception("External API failure"));

        Func<Task> act = async () => await service.HandleGenerateReportCommand(command);

        await act.Should().ThrowAsync<Exception>().WithMessage("External API failure");

        report.Status.Should().Be(ReportStatus.Failed);

        _uowMock.Verify(uow => uow.CompleteAsync(), Times.Exactly(2));
    }
}
