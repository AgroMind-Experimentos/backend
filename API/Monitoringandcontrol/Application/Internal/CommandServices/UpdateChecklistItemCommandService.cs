namespace EcotrackPlatform.API.Monitoringandcontrol.Application.Internal.CommandServices;

using EcotrackPlatform.API.Monitoringandcontrol.Domain.Repositories;
using System.Threading.Tasks;

public class UpdateChecklistItemCommandService
{
    private readonly IChecklistRepository _checklistRepository;

    public UpdateChecklistItemCommandService(IChecklistRepository checklistRepository)
    {
        _checklistRepository = checklistRepository;
    }

    public async Task Handle(int itemId, bool isCompleted)
    {
        var item = await _checklistRepository.GetItemByIdAsync(itemId);
        if (item == null) throw new KeyNotFoundException($"ChecklistItem {itemId} not found");
        item.SetCompleted(isCompleted);
        await _checklistRepository.UpdateItemAsync(item);
    }
}
