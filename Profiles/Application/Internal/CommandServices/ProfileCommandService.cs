using EcotrackPlatform.API.Iam.Domain.Model.Commands;
using EcotrackPlatform.API.Profiles.Domain.Model.Aggregates;
using EcotrackPlatform.API.Profiles.Domain.Model.Commands;
using EcotrackPlatform.API.Profiles.Domain.Repositories;
using EcotrackPlatform.API.Shared.Domain.Repositories;
using Microsoft.AspNetCore.Identity;

namespace EcotrackPlatform.API.Profiles.Application.Internal.CommandServices
{
    public class ProfileCommandService(IProfileRepository profiles, IUnitOfWork uow)
    {
        private readonly PasswordHasher<Profile> _hasher = new();

        public async Task<Profile> CreateAsync(CreateProfileCommand command)
        {
            var existing = await profiles.FindByEmailAsync(command.Email);
            if (existing is not null) throw new InvalidOperationException("Email already in use.");

            var hash = _hasher.HashPassword(null, command.Password);
            var profile = new Profile(command.Email, command.DisplayName, hash, command.Role);

            await profiles.AddAsync(profile);
            await uow.CompleteAsync();
            return profile;
        }

        public async Task<Profile?> UpdateAsync(UpdateProfileCommand command)
        {
            var entity = await profiles.FindByIdAsync(command.Id);
            if (entity is null) return null;

            var displayName = command.DisplayName;
            if (!string.IsNullOrWhiteSpace(displayName))
                entity.Rename(displayName);

            var email = command.Email;
            if (!string.IsNullOrWhiteSpace(email))
            {
                var existing = await profiles.FindByEmailAsync(email);

                var isInUseBySomeoneElse = existing is not null && existing.Id == command.Id;
                if (isInUseBySomeoneElse)
                {
                    throw new InvalidOperationException("Email already in use.");
                }

                entity.SetEmail(email);
            }

            profiles.Update(entity);
            await uow.CompleteAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(DeleteProfileCommand command)
        {
            var entity = await profiles.FindByIdAsync(command.Id);
            if (entity is null) return false;
            profiles.Remove(entity);
            await uow.CompleteAsync();
            return true;
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordCommand command)
        {
            var entity = await profiles.FindByIdAsync(command.Id);
            if (entity is null) return false;

            var result = _hasher.VerifyHashedPassword(entity, entity.PasswordHash, command.CurrentPassword);
            if (result == PasswordVerificationResult.Failed) return false;

            var newHash = _hasher.HashPassword(entity, command.NewPassword);
            entity.SetPasswordHash(newHash);
            profiles.Update(entity);
            await uow.CompleteAsync();
            return true;
        }
    }
}
