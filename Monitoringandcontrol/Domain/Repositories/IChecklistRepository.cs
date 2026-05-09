namespace EcotrackPlatform.API.Monitoringandcontrol.Domain.Repositories;

using EcotrackPlatform.API.Monitoringandcontrol.Domain.Model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IChecklistRepository
{
    Task<int> AddAsync(Checklist checklist);
    Task<Checklist?> GetByIdAsync(int id);
    Task<Checklist?> GetByTaskId(int taskId);
    Task RemoveByTaskIdAsync(int taskId);
    Task UpdateItemsAsync(int checklistId, List<string> descriptions);
    Task<ChecklistItem?> GetItemByIdAsync(int id);
    Task UpdateItemAsync(ChecklistItem item);
}