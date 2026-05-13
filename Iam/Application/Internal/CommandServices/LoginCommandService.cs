using EcotrackPlatform.API.Iam.Domain.Model.Aggregates;
using EcotrackPlatform.API.Iam.Domain.Repositories;
using EcotrackPlatform.API.Iam.Domain.Services;
using EcotrackPlatform.API.Profiles.Domain.Model.Aggregates;
using EcotrackPlatform.API.Profiles.Domain.Repositories;
using EcotrackPlatform.API.Shared.Domain.Repositories;
using Microsoft.AspNetCore.Identity;

namespace EcotrackPlatform.API.Iam.Application.Internal.CommandServices;

public enum LoginError
{
    None,
    InvalidCredentials
}

public record LoginResult(AuthSession? Session = null, string? Token = null, Profile? User = null, LoginError Error = LoginError.None)
{
    public bool Success => Error == LoginError.None;
}

public class LoginCommandService(
    IProfileRepository profiles,
    IAuthSessionRepository sessions,
    IUnitOfWork uow,
    ITokenService tokenService)
{
    private readonly PasswordHasher<Profile> _hasher = new();

    public async Task<LoginResult> LoginAsync(string email, string password, string? ua, string? ip)
    {
        var user = await profiles.FindByEmailAsync(email);
        if (user is null)
            return new LoginResult(Error: LoginError.InvalidCredentials);

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result == PasswordVerificationResult.Failed)
            return new LoginResult(Error: LoginError.InvalidCredentials);

        var session = new AuthSession(user.Id, TimeSpan.FromHours(8), ua, ip);
        await sessions.AddAsync(session);
        await uow.CompleteAsync();

        var token = tokenService.GenerateToken(user);
        return new LoginResult(session, token, user);
    }
}