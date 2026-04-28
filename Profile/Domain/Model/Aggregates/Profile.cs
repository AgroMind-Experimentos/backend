using EcotrackPlatform.API.Organization.Domain.Model.Entities;
using EcotrackPlatform.API.Profile.Domain.Model.ValueObjects;

namespace EcotrackPlatform.API.Profile.Domain.Model.Aggregates
{
    public class Profile
    {
        public int Id { get; private set; }
        public string Email { get; private set; } = default!;
        public string DisplayName { get; private set; } = default!;
        public string PasswordHash { get; private set; } = default!;
        
        public UserRole Role { get; private set; }
        
        private readonly List<OrganizationMember> _memberships = new();
        public IReadOnlyCollection<OrganizationMember> Memberships => _memberships.AsReadOnly();

        protected Profile() { } // EF

        public Profile(string email, string displayName, string passwordHash, UserRole role)
        {
            SetEmail(email);
            Rename(displayName);
            SetPasswordHash(passwordHash);
            Role = role;
        }

        public void Rename(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName)) throw new ArgumentException("DisplayName is required.");
            DisplayName = newName.Trim();
        }

        public void SetEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required.");
            Email = email.Trim().ToLowerInvariant();
        }

        public void SetPasswordHash(string hash)
        {
            if (string.IsNullOrWhiteSpace(hash)) throw new ArgumentException("PasswordHash is required.");
            PasswordHash = hash;
        }
    }
}