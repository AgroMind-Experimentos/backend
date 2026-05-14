using EcotrackPlatform.API.Profiles.Domain.Model.Aggregates;
using EcotrackPlatform.API.Profiles.Domain.Model.ValueObjects;
using EcotrackPlatform.API.Profiles.Domain.Repositories;
using EcotrackPlatform.API.Shared.Domain.Repositories;
using Microsoft.AspNetCore.Identity;

namespace EcotrackPlatform.API.Iam.Application.Internal.CommandServices;

public enum ChangePasswordError
{
    None,
    InvalidInput,
    InsecurePassword,
    ProfileNotFound,
    InvalidCurrentPassword
}

public record ChangePasswordResult(ChangePasswordError Error = ChangePasswordError.None)
{
    public bool Success => Error == ChangePasswordError.None;
}

public class ChangePasswordCommandService(IProfileRepository profiles, IUnitOfWork uow)
{
    private readonly PasswordHasher<Profile> _hasher = new();

    public async Task<ChangePasswordResult> ChangePasswordAsync(int profileId, string currentPassword, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword))
            return new ChangePasswordResult(Error: ChangePasswordError.InvalidInput);

        try
        {
            var _password = new Password(newPassword);
        }
        catch (ArgumentException)
        {
            return new ChangePasswordResult(Error: ChangePasswordError.InsecurePassword);
        }

        var entity = await profiles.FindByIdAsync(profileId);
        if (entity is null)
            return new ChangePasswordResult(Error: ChangePasswordError.ProfileNotFound);

        var result = _hasher.VerifyHashedPassword(entity, entity.PasswordHash, currentPassword);
        if (result == PasswordVerificationResult.Failed)
            return new ChangePasswordResult(Error: ChangePasswordError.InvalidCurrentPassword);

        var newHash = _hasher.HashPassword(entity, newPassword);
        entity.SetPasswordHash(newHash);

        profiles.Update(entity);
        await uow.CompleteAsync();

        return new ChangePasswordResult();
    }
}