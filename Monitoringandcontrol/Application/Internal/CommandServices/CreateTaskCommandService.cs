using EcotrackPlatform.API.Organizations.Domain.Repositories;

namespace EcotrackPlatform.API.Monitoringandcontrol.Application.Internal.CommandServices;

using EcotrackPlatform.API.Monitoringandcontrol.Domain.Model.Aggregates;
using EcotrackPlatform.API.Monitoringandcontrol.Domain.Repositories;
using System.Threading.Tasks;

public class CreateTaskCommandService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IPlotRepository _plotRepository;
    private readonly IOrganizationRepository _organizationRepository;

    public CreateTaskCommandService(ITaskRepository taskRepository, IPlotRepository plotRepository, IOrganizationRepository organizationRepository)
    {
        _taskRepository = taskRepository;
        _plotRepository = plotRepository;
        _organizationRepository = organizationRepository;
    }

    public async Task<int> Handle(string title, string description, int organizationId, int plotId, int responsibleId)
    {
        var organization = await _organizationRepository.FindByIdAsync(organizationId);
        if (organization is null)
            throw new InvalidOperationException($"Organization with id {organizationId} does not exist.");

        var plot = await _plotRepository.FindByIdAsync(plotId);
        if (plot is null)
            throw new InvalidOperationException($"Plot with id {plotId} does not exist.");

        var task = new TaskAggregate(title, description, organizationId, plotId, responsibleId);
        return await _taskRepository.AddAsync(task);
    }
}