namespace EcotrackPlatform.API.Monitoringandcontrol.Application.Internal.CommandServices;

using EcotrackPlatform.API.Monitoringandcontrol.Domain.Model.Aggregates;
using EcotrackPlatform.API.Monitoringandcontrol.Domain.Repositories;
using EcotrackPlatform.API.Organization.Domain.Repositories;
using System.Threading.Tasks;

public class CreateTaskCommandService
{
    private readonly ITaskRepository _taskRepository;
    private readonly ICropRepository _cropRepository;
    private readonly IOrganizationRepository _organizationRepository;

    public CreateTaskCommandService(ITaskRepository taskRepository, ICropRepository cropRepository, IOrganizationRepository organizationRepository)
    {
        _taskRepository = taskRepository;
        _cropRepository = cropRepository;
        _organizationRepository = organizationRepository;
    }

    public async Task<int> Handle(string title, string description, int organizationId, int cropId, int responsibleId)
    {
        var organization = await _organizationRepository.FindByIdAsync(organizationId);
        if (organization is null)
            throw new InvalidOperationException($"Organization with id {organizationId} does not exist.");

        var crop = await _cropRepository.FindByIdAsync(cropId);
        if (crop is null)
            throw new InvalidOperationException($"Crop with id {cropId} does not exist.");

        var task = new TaskAggregate(title, description, organizationId, cropId, responsibleId);
        return await _taskRepository.AddAsync(task);
    }
}