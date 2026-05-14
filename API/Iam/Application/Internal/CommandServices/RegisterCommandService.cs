using EcotrackPlatform.API.Profiles.Domain.Model.Aggregates;
using EcotrackPlatform.API.Profiles.Domain.Model.ValueObjects;
using EcotrackPlatform.API.Profiles.Domain.Repositories;
using EcotrackPlatform.API.Shared.Domain.Repositories;
using Microsoft.AspNetCore.Identity;

namespace EcotrackPlatform.API.Iam.Application.Internal.CommandServices;

public enum RegisterError
{
    None,
    InvalidInput,
    InsecurePassword,
    EmailAlreadyExists
}

public record RegisterResult(Profile? Profile = null, RegisterError Error = RegisterError.None)
{
    public bool Success => Error == RegisterError.None;
}

public class RegisterCommandService(IProfileRepository profiles, IUnitOfWork uow)
{
    private readonly PasswordHasher<Profile> _hasher = new();

    public async Task<RegisterResult> RegisterAsync(string email, string password, string displayName, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(displayName))
            return new RegisterResult(Error: RegisterError.InvalidInput);

        try
        {
            var _password = new Password(password);
        }
        catch (ArgumentException ex)
        {
            return new RegisterResult(Error: RegisterError.InsecurePassword);
        }

        var existing = await profiles.FindByEmailAsync(email);
        if (existing is not null)
            return new RegisterResult(Error: RegisterError.EmailAlreadyExists);

        var temp = new Profile(email, displayName, "temp", role);
        var hash = _hasher.HashPassword(temp, password);

        var profile = new Profile(email, displayName, hash, role);
        await profiles.AddAsync(profile);
        await uow.CompleteAsync();

        return new RegisterResult(Profile: profile);
    }
}