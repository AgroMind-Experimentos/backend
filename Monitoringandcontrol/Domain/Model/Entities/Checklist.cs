namespace EcotrackPlatform.API.Monitoringandcontrol.Domain.Model.Entities;

using System;
using System.Collections.Generic;

public class Checklist
{
    public int Id { get; private set; }
    public int TaskId { get; private set; }
    public string Title { get; private set; }
    
    private readonly List<ChecklistItem> _items = new();
    public List<ChecklistItem> Items => _items;
    
    private Checklist(){}

    public Checklist(int taskId, string title)
    {
        TaskId = taskId;
        Title = title ?? throw new ArgumentNullException(nameof(title));
    }

    public void AddItem(string description)
    {
        _items.Add(new ChecklistItem(description));
    }
}