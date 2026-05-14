using EcotrackPlatform.API.Organizations.Application.Internal.CommandServices;
using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Organizations.Domain.Model.Entities;
using EcotrackPlatform.API.Organizations.Domain.Repositories;
using EcotrackPlatform.API.Profiles.Domain.Model.Aggregates;
using EcotrackPlatform.API.Profiles.Domain.Model.ValueObjects;
using EcotrackPlatform.API.Profiles.Domain.Repositories;
using EcotrackPlatform.API.Shared.Domain.Repositories;

namespace EcotrackPlatform.Tests.Organizations.Application.Internal.CommandServices;

[TestFixture]
public class InvitationCommandServiceTests
{
    private Mock<IInvitationRepository> _invitationRepositoryMock;
    private Mock<IOrganizationRepository> _organizationRepositoryMock;
    private Mock<IProfileRepository> _profileRepositoryMock;
    private Mock<IUnitOfWork> _uowMock;

    [SetUp]
    public void Setup()
    {
        _invitationRepositoryMock = new Mock<IInvitationRepository>();
        _organizationRepositoryMock = new Mock<IOrganizationRepository>();
        _profileRepositoryMock = new Mock<IProfileRepository>();
        _uowMock = new Mock<IUnitOfWork>();
    }

    [Test]
    public async Task SendInvitationByEmailAsync_ValidEmail_ShouldReturnSuccess()
    {
        var service = new InvitationCommandService(
            _invitationRepositoryMock.Object,
            _organizationRepositoryMock.Object,
            _profileRepositoryMock.Object,
            _uowMock.Object);

        var orgId = 1;
        var email = "farmer@test.com";
        var agronomistId = 5;
        var farmer = new Profile(email, "Farmer John", "hash", UserRole.Farmer);

        _profileRepositoryMock.Setup(repo => repo.FindByEmailAsync(email)).ReturnsAsync(farmer);
        _invitationRepositoryMock.Setup(repo => repo.ExistsAsync(orgId, farmer.Id)).ReturnsAsync(false);

        var result = await service.SendInvitationByEmailAsync(orgId, email, agronomistId);

        Assert.That(result, Is.EqualTo(InviteResult.Success));
        _invitationRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Invitation>()), Times.Once);
        _uowMock.Verify(uow => uow.CompleteAsync(), Times.Once);
    }

    [TestCase("")]
    [TestCase("invalidemail")]
    [TestCase("no-at-sign.com")]
    public async Task SendInvitationByEmailAsync_InvalidEmail_ShouldReturnInvalidEmail(string invalidEmail)
    {
        var service = new InvitationCommandService(
            _invitationRepositoryMock.Object,
            _organizationRepositoryMock.Object,
            _profileRepositoryMock.Object,
            _uowMock.Object);

        var result = await service.SendInvitationByEmailAsync(1, invalidEmail, 5);

        Assert.That(result, Is.EqualTo(InviteResult.InvalidEmail));
    }

    [Test]
    public async Task AcceptAsync_ValidPendingInvitation_ShouldAddMemberAndAccept()
    {
        var service = new InvitationCommandService(
            _invitationRepositoryMock.Object,
            _organizationRepositoryMock.Object,
            _profileRepositoryMock.Object,
            _uowMock.Object);

        var invitationId = 10;
        var farmerProfileId = 7;
        var orgId = 1;

        var invitation = new Invitation(orgId, farmerProfileId, 5); // Status defaults to Pending
        var org = new Organization("Org", "Desc", "Loc", 5);

        _invitationRepositoryMock.Setup(repo => repo.FindByIdAsync(invitationId)).ReturnsAsync(invitation);
        _organizationRepositoryMock.Setup(repo => repo.FindByIdWithMembersAsync(orgId)).ReturnsAsync(org);

        var result = await service.AcceptAsync(invitationId, farmerProfileId);

        Assert.That(result, Is.True);
        Assert.That(invitation.Status, Is.EqualTo(InvitationStatus.Accepted));
        Assert.That(org.Members.Any(m => m.ProfileId == farmerProfileId), Is.True);
        
        _organizationRepositoryMock.Verify(repo => repo.Update(org), Times.Once);
        _invitationRepositoryMock.Verify(repo => repo.Update(invitation), Times.Once);
        _uowMock.Verify(uow => uow.CompleteAsync(), Times.Once);
    }

    [Test]
    public async Task RejectAsync_ValidPendingInvitation_ShouldReject()
    {
        var service = new InvitationCommandService(
            _invitationRepositoryMock.Object,
            _organizationRepositoryMock.Object,
            _profileRepositoryMock.Object,
            _uowMock.Object);

        var invitationId = 10;
        var farmerProfileId = 7;
        var orgId = 1;

        var invitation = new Invitation(orgId, farmerProfileId, 5);

        _invitationRepositoryMock.Setup(repo => repo.FindByIdAsync(invitationId)).ReturnsAsync(invitation);

        var result = await service.RejectAsync(invitationId, farmerProfileId);

        Assert.That(result, Is.True);
        Assert.That(invitation.Status, Is.EqualTo(InvitationStatus.Rejected));
        
        _invitationRepositoryMock.Verify(repo => repo.Update(invitation), Times.Once);
        _uowMock.Verify(uow => uow.CompleteAsync(), Times.Once);
    }
}
