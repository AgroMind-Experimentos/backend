using Microsoft.AspNetCore.Identity;
using EcotrackPlatform.API.Iam.Domain.Model.Aggregates;
using EcotrackPlatform.API.Iam.Domain.Repositories;
using EcotrackPlatform.API.Iam.Domain.Services;
using EcotrackPlatform.API.Profiles.Domain.Model.ValueObjects;
using EcotrackPlatform.API.Profiles.Domain.Repositories;
using EcotrackPlatform.API.Shared.Domain.Repositories;
using EcotrackPlatform.API.Profiles.Domain.Model.Aggregates;

namespace EcotrackPlatform.API.Iam.Application.Internal.CommandServices;

public record LoginResult(AuthSession Session, string Token, Profile User);

public record RegisterResult(Profile? Profile, bool EmailConflict = false);

public class AuthCommandService
{
    private readonly IProfileRepository _profiles;
    private readonly IAuthSessionRepository _sessions;
    private readonly IUnitOfWork _uow;
    private readonly ITokenService _tokenService;
    private readonly PasswordHasher<Profile> _hasher = new();

    public AuthCommandService(
        IProfileRepository profiles,
        IAuthSessionRepository sessions,
        IUnitOfWork uow,
        ITokenService tokenService)
    {
        _profiles = profiles;
        _sessions = sessions;
        _uow = uow;
        _tokenService = tokenService;
    }

    public async Task<RegisterResult> RegisterAsync(string email, string password, string displayName, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(displayName))
            return new RegisterResult(null);

        if (password.Length < 6) return new RegisterResult(null);

        var existing = await _profiles.FindByEmailAsync(email);
        if (existing is not null) return new RegisterResult(null, EmailConflict: true);

        var temp = new Profile(email, displayName, "temp", role);
        var hash = _hasher.HashPassword(temp, password);

        var profile = new Profile(email, displayName, hash, role);
        await _profiles.AddAsync(profile);
        await _uow.CompleteAsync();
        return new RegisterResult(profile);
    }

    public async Task<LoginResult?> LoginAsync(string email, string password, string? ua, string? ip)
    {
        var user = await _profiles.FindByEmailAsync(email);
        if (user is null) return null;

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result == PasswordVerificationResult.Failed) return null;

        var session = new AuthSession(user.Id, TimeSpan.FromHours(8), ua, ip);
        await _sessions.AddAsync(session);
        await _uow.CompleteAsync();

        var token = _tokenService.GenerateToken(user);
        return new LoginResult(session, token, user);
    }

    public async Task<bool> LogoutAsync(Guid sessionId)
    {
        var s = await _sessions.FindByIdAsync(sessionId);
        if (s is null || !s.IsActive()) return false;
        s.Revoke();
        _sessions.Update(s);
        await _uow.CompleteAsync();
        return true;
    }
}
