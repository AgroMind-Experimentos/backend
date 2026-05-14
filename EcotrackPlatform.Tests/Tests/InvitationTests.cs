using EcotrackPlatform.API.Organizations.Domain.Model.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace EcotrackPlatform.Tests;

[TestFixture]
public class InvitationTests
{
    [Test]
    public void CreateInvitation_WithValidData_ShouldInitializeAsPending()
    {
        // Arrange
        var orgId = 1;
        var farmerId = 10;
        var agronomistId = 5;

        // Act
        var invitation = new Invitation(orgId, farmerId, agronomistId);

        // Assert
        invitation.OrganizationId.Should().Be(orgId);
        invitation.FarmerProfileId.Should().Be(farmerId);
        invitation.AgronomistProfileId.Should().Be(agronomistId);
        invitation.Status.Should().Be(InvitationStatus.Pending);
        invitation.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Test]
    public void AcceptInvitation_WhenCalled_ShouldChangeStatusToAccepted()
    {
        // Arrange
        var invitation = new Invitation(1, 10, 5);

        // Act
        invitation.Accept();

        // Assert
        invitation.Status.Should().Be(InvitationStatus.Accepted);
    }

    [Test]
    public void RejectInvitation_WhenCalled_ShouldChangeStatusToRejected()
    {
        // Arrange
        var invitation = new Invitation(1, 10, 5);

        // Act
        invitation.Reject();

        // Assert
        invitation.Status.Should().Be(InvitationStatus.Rejected);
    }
}