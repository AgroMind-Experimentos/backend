using Microsoft.AspNetCore.Mvc;
using EcotrackPlatform.API.Monitoringandcontrol.Application.Internal.CommandServices;
using EcotrackPlatform.API.Monitoringandcontrol.Application.Internal.QueryServices;
using EcotrackPlatform.API.Monitoringandcontrol.Interfaces.REST.Resources;
using EcotrackPlatform.API.Monitoringandcontrol.Interfaces.REST.Transform;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;
using System.Threading.Tasks;
using EcotrackPlatform.API.Monitoringandcontrol.Domain.Model.Entities;
using EcotrackPlatform.API.Monitoringandcontrol.Interfaces.REST.Resources.Requests;

namespace EcotrackPlatform.API.Monitoringandcontrol.Interfaces.REST.Controllers;


[ApiController]
[Route("api/checklists")]
[Tags("Checklists")]
public class ChecklistController : ControllerBase
{
    private readonly CreateChecklistCommandService _createChecklistCommandService;
    private readonly UpdateChecklistCommandService _updateChecklistCommandService;
    private readonly UpdateChecklistItemCommandService _updateChecklistItemCommandService;
    private readonly GetChecklistByTaskIdQueryService _getChecklistByTaskIdQueryService;

    public ChecklistController(CreateChecklistCommandService createChecklistCommandService,
        UpdateChecklistCommandService updateChecklistCommandService,
        UpdateChecklistItemCommandService updateChecklistItemCommandService,
        GetChecklistByTaskIdQueryService getChecklistByTaskIdQueryService)
    {
        _createChecklistCommandService = createChecklistCommandService;
        _updateChecklistCommandService = updateChecklistCommandService;
        _updateChecklistItemCommandService = updateChecklistItemCommandService;
        _getChecklistByTaskIdQueryService = getChecklistByTaskIdQueryService;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new Checklist")]
    public async Task<IActionResult> Create([FromBody] CreateChecklistRequest request)
    {
        var items = request.Items.Select(i => i.Description).ToList();
        var checklistId = await _createChecklistCommandService.Handle(request.TaskId, request.Title, items);
        return CreatedAtAction(nameof(GetByTaskId), new { Id = checklistId }, new { Id = checklistId });
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get checklists by Task Id")]
    public async Task<IActionResult> GetByTaskId(int taskId)
    {
        var checklist = await _getChecklistByTaskIdQueryService.Handle(taskId);
        if (checklist == null)
        {
            return Ok(new { message = "Checklist not found" });
        }
        return Ok(ChecklistAssembler.ToResource(checklist));
    }

    [HttpPatch("items/{itemId:int}")]
    [SwaggerOperation(Summary = "Mark or unmark a checklist item as completed")]
    public async Task<IActionResult> UpdateItemCompletion(int itemId, [FromBody] UpdateChecklistItemRequest request)
    {
        try
        {
            await _updateChecklistItemCommandService.Handle(itemId, request.IsCompleted);
            return Ok(new { message = "Item updated" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Item not found" });
        }
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Update checklist items")]
    public async Task<IActionResult> UpdateChecklist(int id, [FromBody] UpdateChecklistRequest request)
    {
        try
        {
            var descriptions = request.Items.Select(i => i.Description).ToList();
            await _updateChecklistCommandService.Handle(id, descriptions);
            return Ok(new { message = "Checklist updated" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Checklist not found" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
