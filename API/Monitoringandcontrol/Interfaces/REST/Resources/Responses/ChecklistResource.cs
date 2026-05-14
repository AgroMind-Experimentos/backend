namespace EcotrackPlatform.API.Monitoringandcontrol.Interfaces.REST.Resources.Responses;

public record ChecklistResource(string Id, int TaskId, string Title, List<ChecklistItemResource> Items);