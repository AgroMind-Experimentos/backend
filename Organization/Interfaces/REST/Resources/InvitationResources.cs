namespace EcotrackPlatform.API.Organization.Interfaces.REST.Resources;

public record InviteByEmailResource(string Email, int AgronomistId);
public record RespondInvitationResource(int ProfileId);
