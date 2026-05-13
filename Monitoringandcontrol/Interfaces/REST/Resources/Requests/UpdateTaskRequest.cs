namespace EcotrackPlatform.API.Monitoringandcontrol.Interfaces.REST.Resources.Requests;

public record UpdateTaskRequest(string Title, string Description, int ResponsibleId);
