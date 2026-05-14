namespace EcotrackPlatform.API.Profiles.Domain.Model.ValueObjects;

public record Password
{
    public string Value { get; init; }

    public Password(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length < 8)
        {
            throw new ArgumentException("Password must be at least 8 characters long.");
        }

        if (!value.Any(char.IsUpper))
        {
            throw new ArgumentException("Password must contain at least an uppercase letter.");
        }

        if (!value.Any(char.IsLower))
        {
            throw new ArgumentException("Password must contain at least a lowercase letter.");
        }

        if (!value.Any(char.IsDigit))
        {
            throw new ArgumentException("Password must contain at least a numer.");
        }

        Value = value;
    }

    public override string ToString() => "********";
}