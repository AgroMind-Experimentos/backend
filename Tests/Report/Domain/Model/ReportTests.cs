using EcotrackPlatform.API.Report.Domain.Model.ValueObjects;
using ReportEntity = EcotrackPlatform.API.Report.Domain.Model.Report;

namespace EcotrackPlatform.Tests.Report.Domain.Model;

[TestFixture]
public class ReportTests
{
    private PlotId _plotId;
    private ProfileId _profileId;
    private DateTime _periodStart;
    private DateTime _periodEnd;

    [SetUp]
    public void SetUp()
    {
        _plotId = new PlotId(1);
        _profileId = new ProfileId(1);
        _periodStart = new DateTime(2023, 1, 1);
        _periodEnd = new DateTime(2023, 12, 31);
    }

    [Test]
    public void Request_ValidDates_ShouldCreateReport()
    {
        // Act
        var report = ReportEntity.Request(ReportType.ActivitySummary, _plotId, _profileId, _periodStart, _periodEnd);

        // Assert
        Assert.That(report, Is.Not.Null);
        Assert.That(report.Status, Is.EqualTo(ReportStatus.Requested));
        Assert.That(report.Type, Is.EqualTo(ReportType.ActivitySummary));
        Assert.That(report.PlotId, Is.EqualTo(_plotId));
        Assert.That(report.RequestedBy, Is.EqualTo(_profileId));
        Assert.That(report.PeriodStart, Is.EqualTo(_periodStart));
        Assert.That(report.PeriodEnd, Is.EqualTo(_periodEnd));
    }

    [Test]
    public void Request_EndDateBeforeStartDate_ShouldThrowArgumentException()
    {
        // Arrange
        var invalidPeriodEnd = _periodStart.AddDays(-1);

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            ReportEntity.Request(ReportType.ActivitySummary, _plotId, _profileId, _periodStart, invalidPeriodEnd));
        
        Assert.That(ex.Message, Does.Contain("La fecha de fin no puede ser anterior a la fecha de inicio"));
    }

    [Test]
    public void Generate_ValidDataAndState_ShouldSetContentAndStatusGenerated()
    {
        // Arrange
        var report = ReportEntity.Request(ReportType.ActivitySummary, _plotId, _profileId, _periodStart, _periodEnd);
        var data = new byte[] { 1, 2, 3 };

        // Act
        report.Generate(data);

        // Assert
        Assert.That(report.Status, Is.EqualTo(ReportStatus.Generated));
        Assert.That(report.Content, Is.EqualTo(data));
        Assert.That(report.GeneratedAt, Is.Not.Null);
        Assert.That(report.UpdatedAt, Is.Not.Null);
    }

    [Test]
    public void Generate_EmptyData_ShouldThrowArgumentException()
    {
        // Arrange
        var report = ReportEntity.Request(ReportType.ActivitySummary, _plotId, _profileId, _periodStart, _periodEnd);

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => report.Generate(Array.Empty<byte>()));
        
        Assert.That(ex.Message, Does.Contain("Los datos del reporte no pueden estar vacíos"));
    }

    [Test]
    public void Generate_InvalidState_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var report = ReportEntity.Request(ReportType.ActivitySummary, _plotId, _profileId, _periodStart, _periodEnd);
        report.MarkFailed("Error"); // Change state to Failed

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => report.Generate(new byte[] { 1 }));
        
        Assert.That(ex.Message, Does.Contain("No se puede generar un reporte en estado"));
    }

    [Test]
    public void MarkFailed_ValidReason_ShouldSetStatusFailed()
    {
        // Arrange
        var report = ReportEntity.Request(ReportType.ActivitySummary, _plotId, _profileId, _periodStart, _periodEnd);
        var reason = "Service unavailable";

        // Act
        report.MarkFailed(reason);

        // Assert
        Assert.That(report.Status, Is.EqualTo(ReportStatus.Failed));
        Assert.That(report.FailureReason, Is.EqualTo(reason));
        Assert.That(report.UpdatedAt, Is.Not.Null);
    }

    [TestCase("")]
    [TestCase(" ")]
    public void MarkFailed_EmptyReason_ShouldThrowArgumentException(string invalidReason)
    {
        // Arrange
        var report = ReportEntity.Request(ReportType.ActivitySummary, _plotId, _profileId, _periodStart, _periodEnd);

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => report.MarkFailed(invalidReason));
        
        Assert.That(ex.Message, Does.Contain("Debe proporcionar una razón para el fallo"));
    }

    [Test]
    public void MarkAsGenerating_ValidState_ShouldSetStatusGenerating()
    {
        // Arrange
        var report = ReportEntity.Request(ReportType.ActivitySummary, _plotId, _profileId, _periodStart, _periodEnd);

        // Act
        report.MarkAsGenerating();

        // Assert
        Assert.That(report.Status, Is.EqualTo(ReportStatus.Generating));
        Assert.That(report.UpdatedAt, Is.Not.Null);
    }

    [Test]
    public void MarkAsGenerating_InvalidState_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var report = ReportEntity.Request(ReportType.ActivitySummary, _plotId, _profileId, _periodStart, _periodEnd);
        report.MarkAsGenerating(); // State is now Generating

        // Act & Assert - Calling MarkAsGenerating again from Generating state
        var ex = Assert.Throws<InvalidOperationException>(() => report.MarkAsGenerating());
        
        Assert.That(ex.Message, Does.Contain("No se puede comenzar a generar un reporte en estado"));
    }
}
