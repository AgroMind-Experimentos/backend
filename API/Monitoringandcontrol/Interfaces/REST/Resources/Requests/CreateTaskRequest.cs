namespace EcotrackPlatform.API.Monitoringandcontrol.Interfaces.REST.Resources.Requests;

public record CreateTaskRequest(string Title, string Description, int OrganizationId, int PlotId, int ResponsibleId);