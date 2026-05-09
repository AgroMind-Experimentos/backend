using EcotrackPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace EcotrackPlatform.API.Monitoringandcontrol.Infraestructure.Persistence.EFC.Respositories;

using Microsoft.EntityFrameworkCore;
using EcotrackPlatform.API.Monitoringandcontrol.Domain.Model.Entities;
using EcotrackPlatform.API.Monitoringandcontrol.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ChecklistRepository : IChecklistRepository
{
    private readonly AppDbContext _dbContext;

    public ChecklistRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> AddAsync(Checklist checklist)
    {
        await _dbContext.Set<Checklist>().AddAsync(checklist);
        await _dbContext.SaveChangesAsync();
        return checklist.Id;
    }

    public async Task<Checklist?> GetByTaskId(int taskId)
    {
        return await _dbContext.Set<Checklist>().Include(c => c.Items).FirstOrDefaultAsync(x => x.TaskId == taskId);
    }

    public async Task<Checklist?> GetByIdAsync(int id)
    {
        return await _dbContext.Set<Checklist>().Include(c => c.Items).FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task RemoveByTaskIdAsync(int taskId)
    {
        var checklist = await _dbContext.Set<Checklist>().Include(c => c.Items).FirstOrDefaultAsync(x => x.TaskId == taskId);
        if (checklist == null) return;
        _dbContext.Set<Checklist>().Remove(checklist);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateItemsAsync(int checklistId, List<string> descriptions)
    {
        var checklist = await _dbContext.Set<Checklist>().Include(c => c.Items).FirstOrDefaultAsync(x => x.Id == checklistId);
        if (checklist == null) throw new KeyNotFoundException($"Checklist {checklistId} not found");

        foreach (var item in checklist.Items.ToList())
            _dbContext.Remove(item);

        checklist.ClearItems();

        foreach (var desc in descriptions)
            checklist.AddItem(desc);

        await _dbContext.SaveChangesAsync();
    }
}