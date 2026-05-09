namespace EcotrackPlatform.API.Monitoringandcontrol.Application.Internal.CommandServices;

using EcotrackPlatform.API.Monitoringandcontrol.Domain.Repositories;
using System.Threading.Tasks;

public class DeleteTaskCommandService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IChecklistRepository _checklistRepository;

    public DeleteTaskCommandService(ITaskRepository taskRepository, IChecklistRepository checklistRepository)
    {
        _taskRepository = taskRepository;
        _checklistRepository = checklistRepository;
    }

    public async Task<bool> Handle(int taskId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null) return false;

        await _checklistRepository.RemoveByTaskIdAsync(taskId);
        await _taskRepository.DeleteAsync(taskId);
        return true;
    }
}
