namespace EcotrackPlatform.API.Monitoringandcontrol.Application.Internal.CommandServices;

using EcotrackPlatform.API.Monitoringandcontrol.Domain.Repositories;
using System.Threading.Tasks;

public class UpdateTaskCommandService
{
    private readonly ITaskRepository _taskRepository;

    public UpdateTaskCommandService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task Handle(int taskId, string title, string description, int responsibleId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null) throw new KeyNotFoundException($"Task {taskId} not found");

        task.UpdateDetails(title, description, responsibleId);
        await _taskRepository.UpdateAsync(task);
    }
}
