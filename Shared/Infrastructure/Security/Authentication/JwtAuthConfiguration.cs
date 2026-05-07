namespace EcotrackPlatform.API.Shared.Infrastructure.Security.Authentication;

public readonly record struct JwtAuthConfiguration(
    string SecretKey,
    string Issuer,
    string Audience
);