using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;

namespace EcotrackPlatform.Tests.Organizations.Domain.Model;

[TestFixture]
public class OrganizationTests
{
    private const string ValidName = "Finca El Sol";
    private const string ValidDescription = "Producción agrícola orgánica";
    private const string ValidLocation = "Región Central";
    private const int AgronomistOwnerId = 42;

    [Test]
    public void Constructor_ValidArguments_ShouldCreateOrganization()
    {
        // Act
        var org = new Organization(ValidName, ValidDescription, ValidLocation, AgronomistOwnerId);

        // Assert
        Assert.That(org, Is.Not.Null);
        Assert.That(org.Name, Is.EqualTo(ValidName));
        Assert.That(org.Description, Is.EqualTo(ValidDescription));
        Assert.That(org.Location, Is.EqualTo(ValidLocation));
        Assert.That(org.AgronomistOwnerId, Is.EqualTo(AgronomistOwnerId));
        Assert.That(org.CreatedAt, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(1)));
        Assert.That(org.Members, Is.Empty);
        Assert.That(org.Plots, Is.Empty);
    }

    [TestCase("")]
    [TestCase(" ")]
    public void Constructor_InvalidName_ShouldThrowArgumentException(string invalidName)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            new Organization(invalidName, ValidDescription, ValidLocation, AgronomistOwnerId));
        Assert.That(ex.Message, Does.Contain("Name cannot be empty or whitespace"));
    }

    [Test]
    public void Update_ValidArguments_ShouldUpdateProperties()
    {
        // Arrange
        var org = new Organization(ValidName, ValidDescription, ValidLocation, AgronomistOwnerId);
        var newName = "Hacienda La Luna";
        var newDescription = "Producción de cereales";
        var newLocation = "Región Norte";

        // Act
        org.Update(newName, newDescription, newLocation);

        // Assert
        Assert.That(org.Name, Is.EqualTo(newName));
        Assert.That(org.Description, Is.EqualTo(newDescription));
        Assert.That(org.Location, Is.EqualTo(newLocation));
    }

    [Test]
    public void Update_NullArguments_ShouldNotUpdateProperties()
    {
        // Arrange
        var org = new Organization(ValidName, ValidDescription, ValidLocation, AgronomistOwnerId);

        // Act
        org.Update(null, null, null);

        // Assert
        Assert.That(org.Name, Is.EqualTo(ValidName));
        Assert.That(org.Description, Is.EqualTo(ValidDescription));
        Assert.That(org.Location, Is.EqualTo(ValidLocation));
    }

    [TestCase("")]
    [TestCase(" ")]
    public void Update_InvalidDescription_ShouldThrowArgumentException(string invalidDescription)
    {
        // Arrange
        var org = new Organization(ValidName, ValidDescription, ValidLocation, AgronomistOwnerId);

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            org.Update(null, invalidDescription, null));
        Assert.That(ex.Message, Does.Contain("Description cannot be empty or whitespace"));
    }

    [Test]
    public void SyncMembers_WithDuplicateProfileIds_ShouldAddDistinctMembersOnly()
    {
        // Arrange
        var org = new Organization(ValidName, ValidDescription, ValidLocation, AgronomistOwnerId);
        var profileIds = new[] { 1, 2, 2, 3, 1 };

        // Act
        org.SyncMembers(profileIds);

        // Assert
        Assert.That(org.Members, Has.Count.EqualTo(3));
        Assert.That(org.Members.Select(m => m.ProfileId), Is.EquivalentTo(new[] { 1, 2, 3 }));
        Assert.That(org.Members.All(m => m.OrganizationId == org.Id), Is.True);
    }

    [Test]
    public void SyncMembers_EmptyList_ShouldClearExistingMembers()
    {
        // Arrange
        var org = new Organization(ValidName, ValidDescription, ValidLocation, AgronomistOwnerId);
        org.SyncMembers(new[] { 1, 2 });
        Assert.That(org.Members, Is.Not.Empty);

        // Act
        org.SyncMembers(Array.Empty<int>());

        // Assert
        Assert.That(org.Members, Is.Empty);
    }
}
