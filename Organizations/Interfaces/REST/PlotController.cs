using EcotrackPlatform.API.Organizations.Application.Services;
using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Organizations.Domain.Model.Queries;
using EcotrackPlatform.API.Organizations.Interfaces.REST.Resources;
using EcotrackPlatform.API.Organizations.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;

namespace EcotrackPlatform.API.Organizations.Interfaces.REST;

[ApiController]
[Route("api/v1/plots")]
public class PlotsController : ControllerBase
{
    private readonly IPlotCommandService _commandService;
    private readonly IPlotQueryService _queryService;

    public PlotsController(IPlotCommandService commandService, IPlotQueryService queryService)
    {
        _commandService = commandService;
        _queryService = queryService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePlotResource resource)
    {
        try
        {
            var command = CreatePlotCommandFromResourceAssembler.ToCommand(resource);
            Plot result = await _commandService.Handle(command);
            var resourceResult = PlotResourceFromEntityAssembler.ToResource(result);
            return CreatedAtAction(nameof(GetById), new { id = resourceResult.Id }, resourceResult);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdatePlotResource resource)
    {
        try
        {
            var updated = await _commandService.UpdateAsync(id, resource.Name, resource.Location, resource.Area, resource.Cultivation, resource.MemberIds);
            if (updated is null) return NotFound();

            return Ok(PlotResourceFromEntityAssembler.ToResource(updated));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetAllPlotsQuery();
        var plots = await _queryService.Handle(query);
        var resources = plots.Select(PlotResourceFromEntityAssembler.ToResource);
        return Ok(resources);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GetPlotByIdQuery(id);
        var plot = await _queryService.Handle(query);
        
        if (plot == null)
            return NotFound();
        
        var resource = PlotResourceFromEntityAssembler.ToResource(plot);
        return Ok(resource);
    }

    [HttpGet("organization/{organizationId}")]
    public async Task<IActionResult> GetByOrganizationId(int organizationId)
    {
        var query = new GetAllPlotsByOrganizationIdQuery(organizationId);
        var plots = await _queryService.Handle(query);
        var resources = plots.Select(PlotResourceFromEntityAssembler.ToResource);
        return Ok(resources);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _commandService.Handle(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}