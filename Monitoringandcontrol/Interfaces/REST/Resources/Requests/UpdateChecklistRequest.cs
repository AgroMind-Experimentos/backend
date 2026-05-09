namespace EcotrackPlatform.API.Monitoringandcontrol.Interfaces.REST.Resources.Requests;

public record UpdateChecklistRequest(List<ChecklistItemRequest> Items);
