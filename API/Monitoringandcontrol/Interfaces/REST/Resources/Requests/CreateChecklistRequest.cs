namespace EcotrackPlatform.API.Monitoringandcontrol.Interfaces.REST.Resources.Requests;

public record CreateChecklistRequest(int TaskId, string Title, List<ChecklistItemRequest> Items);