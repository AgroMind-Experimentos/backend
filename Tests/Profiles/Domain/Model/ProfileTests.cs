using EcotrackPlatform.API.Profiles.Domain.Model.Aggregates;
using EcotrackPlatform.API.Profiles.Domain.Model.ValueObjects;

namespace EcotrackPlatform.Tests.Profiles.Domain.Model;

[TestFixture]
public class ProfileTests
{
    private const string ValidEmail = "test@example.com";
    private const string ValidDisplayName = "John Doe";
    private const string ValidPasswordHash = "hashed_password_123";
    private const UserRole ValidRole = UserRole.Agronomist;

    [Test]
    public void Constructor_ValidArguments_ShouldCreateProfileAndFormatStrings()
    {
        // Arrange
        var unformattedEmail = "  Test@EXAMPLE.com  ";
        var unformattedName = "   John Doe   ";

        // Act
        var profile = new Profile(unformattedEmail, unformattedName, ValidPasswordHash, ValidRole);

        // Assert
        Assert.That(profile, Is.Not.Null);
        Assert.That(profile.Email, Is.EqualTo("test@example.com"), "Email should be trimmed and lowercased.");
        Assert.That(profile.DisplayName, Is.EqualTo("John Doe"), "Display name should be trimmed.");
        Assert.That(profile.PasswordHash, Is.EqualTo(ValidPasswordHash));
        Assert.That(profile.Role, Is.EqualTo(ValidRole));
        Assert.That(profile.Memberships, Is.Not.Null);
    }

    [TestCase("")]
    [TestCase(" ")]
    public void Constructor_InvalidEmail_ShouldThrowArgumentException(string invalidEmail)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            new Profile(invalidEmail, ValidDisplayName, ValidPasswordHash, ValidRole));
        Assert.That(ex.Message, Does.Contain("Email is required."));
    }

    [TestCase("")]
    [TestCase(" ")]
    public void Constructor_InvalidDisplayName_ShouldThrowArgumentException(string invalidName)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            new Profile(ValidEmail, invalidName, ValidPasswordHash, ValidRole));
        Assert.That(ex.Message, Does.Contain("DisplayName is required."));
    }

    [TestCase("")]
    [TestCase(" ")]
    public void Constructor_InvalidPasswordHash_ShouldThrowArgumentException(string invalidHash)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            new Profile(ValidEmail, ValidDisplayName, invalidHash, ValidRole));
        Assert.That(ex.Message, Does.Contain("PasswordHash is required."));
    }

    [Test]
    public void Rename_ValidName_ShouldUpdateDisplayNameAndTrim()
    {
        // Arrange
        var profile = new Profile(ValidEmail, ValidDisplayName, ValidPasswordHash, ValidRole);
        var newName = "  Jane Doe  ";

        // Act
        profile.Rename(newName);

        // Assert
        Assert.That(profile.DisplayName, Is.EqualTo("Jane Doe"));
    }

    [TestCase("")]
    [TestCase(" ")]
    public void Rename_InvalidName_ShouldThrowArgumentException(string invalidName)
    {
        // Arrange
        var profile = new Profile(ValidEmail, ValidDisplayName, ValidPasswordHash, ValidRole);

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => profile.Rename(invalidName));
        Assert.That(ex.Message, Does.Contain("DisplayName is required."));
    }

    [Test]
    public void SetEmail_ValidEmail_ShouldUpdateEmailAndFormat()
    {
        // Arrange
        var profile = new Profile(ValidEmail, ValidDisplayName, ValidPasswordHash, ValidRole);
        var newEmail = "  NEW@Example.COM  ";

        // Act
        profile.SetEmail(newEmail);

        // Assert
        Assert.That(profile.Email, Is.EqualTo("new@example.com"));
    }

    [TestCase("")]
    [TestCase(" ")]
    public void SetEmail_InvalidEmail_ShouldThrowArgumentException(string invalidEmail)
    {
        // Arrange
        var profile = new Profile(ValidEmail, ValidDisplayName, ValidPasswordHash, ValidRole);

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => profile.SetEmail(invalidEmail));
        Assert.That(ex.Message, Does.Contain("Email is required."));
    }

    [Test]
    public void SetPasswordHash_ValidHash_ShouldUpdateHash()
    {
        // Arrange
        var profile = new Profile(ValidEmail, ValidDisplayName, ValidPasswordHash, ValidRole);
        var newHash = "new_hashed_password";

        // Act
        profile.SetPasswordHash(newHash);

        // Assert
        Assert.That(profile.PasswordHash, Is.EqualTo(newHash));
    }

    [TestCase("")]
    [TestCase(" ")]
    public void SetPasswordHash_InvalidHash_ShouldThrowArgumentException(string invalidHash)
    {
        // Arrange
        var profile = new Profile(ValidEmail, ValidDisplayName, ValidPasswordHash, ValidRole);

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => profile.SetPasswordHash(invalidHash));
        Assert.That(ex.Message, Does.Contain("PasswordHash is required."));
    }
}
