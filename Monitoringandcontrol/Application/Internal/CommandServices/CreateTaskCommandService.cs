namespace EcotrackPlatform.API.Monitoringandcontrol.Application.Internal.CommandServices;

using EcotrackPlatform.API.Monitoringandcontrol.Domain.Model.Aggregates;
using EcotrackPlatform.API.Monitoringandcontrol.Domain.Repositories;
using EcotrackPlatform.API.Organization.Domain.Repositories;
using System.Threading.Tasks;

public class CreateTaskCommandService
{
    private readonly ITaskRepository _taskRepository;
    private readonly ICropRepository _cropRepository;

    public CreateTaskCommandService(ITaskRepository taskRepository, ICropRepository cropRepository)
    {
        _taskRepository = taskRepository;
        _cropRepository = cropRepository;
    }

    public async Task<int> Handle(string title, string description, int cropId, int responsibleId)
    {
        var crop = await _cropRepository.FindByIdWithMembersAsync(cropId);
        if (crop is null)
        {
            throw new InvalidOperationException($"Crop with id {cropId} does not exist.");
        }

        if (!crop.Members.Any(member => member.ProfileId == responsibleId))
        {
            throw new InvalidOperationException($"Profile with id {responsibleId} does not belong to crop {cropId}.");
        }

        var task = new TaskAggregate(title, description, cropId, responsibleId);
        return await _taskRepository.AddAsync(task);
    }
}