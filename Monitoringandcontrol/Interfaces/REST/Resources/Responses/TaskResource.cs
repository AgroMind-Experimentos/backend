namespace EcotrackPlatform.API.Monitoringandcontrol.Interfaces.REST.Resources.Responses;

public record TaskResource(string Id, string Title, int ResponsibleId, string Status, DateTime? StartedAt, DateTime? CompletedAt);