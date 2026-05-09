namespace EcotrackPlatform.API.Monitoringandcontrol.Application.Internal.CommandServices;

using EcotrackPlatform.API.Monitoringandcontrol.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class UpdateChecklistCommandService
{
    private readonly IChecklistRepository _checklistRepository;

    public UpdateChecklistCommandService(IChecklistRepository checklistRepository)
    {
        _checklistRepository = checklistRepository;
    }

    public async Task Handle(int checklistId, List<string> descriptions)
    {
        var valid = descriptions.Where(d => !string.IsNullOrWhiteSpace(d)).ToList();
        if (valid.Count == 0)
            throw new InvalidOperationException("El checklist debe tener al menos un ítem válido.");

        await _checklistRepository.UpdateItemsAsync(checklistId, valid);
    }
}
