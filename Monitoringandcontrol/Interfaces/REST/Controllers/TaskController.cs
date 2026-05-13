using Microsoft.AspNetCore.Mvc;
using EcotrackPlatform.API.Monitoringandcontrol.Application.Internal.CommandServices;
using EcotrackPlatform.API.Monitoringandcontrol.Application.Internal.QueryServices;
using EcotrackPlatform.API.Monitoringandcontrol.Interfaces.REST.Transform;
using Swashbuckle.AspNetCore.Annotations;
using EcotrackPlatform.API.Monitoringandcontrol.Interfaces.REST.Resources.Requests;

namespace EcotrackPlatform.API.Monitoringandcontrol.Interfaces.REST.Controllers;

[ApiController]
[Route("api/v1/tasks")]
[Tags("Tasks")]
public class TaskController : ControllerBase
{
    private readonly CreateTaskCommandService _createTaskCommandService;
    private readonly UpdateTaskCommandService _updateTaskCommandService;
    private readonly UpdateTaskStatusCommandService _updateTaskStatusCommandService;
    private readonly GetTasksQueryService _getTasksQueryService;
    private readonly DeleteTaskCommandService _deleteTaskCommandService;

    public TaskController(CreateTaskCommandService createTaskCommandService,
        UpdateTaskCommandService updateTaskCommandService,
        UpdateTaskStatusCommandService updateTaskStatusCommandService,
        GetTasksQueryService getTasksQueryService,
        DeleteTaskCommandService deleteTaskCommandService)
    {
        _createTaskCommandService = createTaskCommandService;
        _updateTaskCommandService = updateTaskCommandService;
        _updateTaskStatusCommandService = updateTaskStatusCommandService;
        _getTasksQueryService = getTasksQueryService;
        _deleteTaskCommandService = deleteTaskCommandService;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new task")]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request)
    {
        try
        {
            var taskId = await _createTaskCommandService.Handle(request.Title, request.Description, request.OrganizationId, request.PlotId, request.ResponsibleId);
            var tasks = await _getTasksQueryService.Handle();
            var task = tasks.Find(t => t.Id == taskId);
            return CreatedAtAction(nameof(GetById), new { taskId }, TaskAssembler.ToCreatedResource(task!));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{taskId}/status")]
    [SwaggerOperation(Summary = "Update a task's status")]
    public async Task<IActionResult> UpdateStatus(int taskId, [FromBody] UpdateStatusRequest request)
    {
        await _updateTaskStatusCommandService.Handle(taskId, request.Status);
        return Ok(new { message = "Status Updated" });
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get tasks filtered by status, organizationId or responsibleId")]
    public async Task<IActionResult> GetTasksByStatus(
        [FromQuery] string? status = null,
        [FromQuery] int? organizationId = null,
        [FromQuery] int? responsibleId = null)
    {
        var tasks = await _getTasksQueryService.Handle();

        if (!string.IsNullOrEmpty(status))
        {
            if (Enum.TryParse<Domain.Model.ValueObjects.TaskStatus>(status, true, out var statusEnum))
                tasks = tasks.Where(t => t.Status == statusEnum).ToList();
            else
                return BadRequest(new { message = "Invalid status" });
        }

        if (organizationId.HasValue)
            tasks = tasks.Where(t => t.OrganizationId == organizationId.Value).ToList();

        if (responsibleId.HasValue)
            tasks = tasks.Where(t => t.ResponsibleId == responsibleId.Value).ToList();

        return Ok(tasks.Select(TaskAssembler.ToResource));
    }

    [HttpGet("{taskId:int}")]
    [SwaggerOperation(Summary = "Get task by id")]
    public async Task<IActionResult> GetById(int taskId)
    {
        var tasks = await _getTasksQueryService.Handle();
        var task = tasks.Find(t => t.Id == taskId);
        if (task == null)
        {
            return NotFound();
        }
        return Ok(TaskAssembler.ToResource(task));
    }

    [HttpPut("{taskId:int}")]
    [SwaggerOperation(Summary = "Update task title and description")]
    public async Task<IActionResult> UpdateTask(int taskId, [FromBody] UpdateTaskRequest request)
    {
        try
        {
            await _updateTaskCommandService.Handle(taskId, request.Title, request.Description, request.ResponsibleId);
            return Ok(new { message = "Task updated" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Task not found" });
        }
    }

    [HttpDelete("{taskId:int}")]
    [SwaggerOperation(Summary = "Delete a task by id")]
    public async Task<IActionResult> DeleteTask(int taskId)
    {
        var deleted = await _deleteTaskCommandService.Handle(taskId);
        if (!deleted) return NotFound(new { message = "Task not found" });
        return NoContent();
    }
}