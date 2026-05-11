using EcotrackPlatform.API.Organizations.Aplication.Services;
using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Organizations.Domain.Model.Queries;
using EcotrackPlatform.API.Organizations.Interfaces.REST.Resources;
using EcotrackPlatform.API.Organizations.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;

namespace EcotrackPlatform.API.Organizations.Interfaces.REST;

[ApiController]
[Route("api/v1/crops")]
public class CropsController : ControllerBase
{
    private readonly ICropCommandService _commandService;
    private readonly ICropQueryService _queryService;

    public CropsController(ICropCommandService commandService, ICropQueryService queryService)
    {
        _commandService = commandService;
        _queryService = queryService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCropResource resource)
    {
        try
        {
            var command = CreateCropCommandFromResourceAssembler.ToCommand(resource);
            Crop result = await _commandService.Handle(command);
            var resourceResult = CropResourceFromEntityAssembler.ToResource(result);
            return CreatedAtAction(nameof(GetById), new { id = resourceResult.Id }, resourceResult);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCropResource resource)
    {
        try
        {
            var updated = await _commandService.UpdateAsync(id, resource.Name, resource.Location, resource.Area, resource.Cultivation, resource.MemberIds);
            if (updated is null) return NotFound();

            return Ok(CropResourceFromEntityAssembler.ToResource(updated));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetAllCropsQuery();
        var crops = await _queryService.Handle(query);
        var resources = crops.Select(CropResourceFromEntityAssembler.ToResource);
        return Ok(resources);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GetCropByIdQuery(id);
        var crop = await _queryService.Handle(query);
        
        if (crop == null)
            return NotFound();
        
        var resource = CropResourceFromEntityAssembler.ToResource(crop);
        return Ok(resource);
    }

    [HttpGet("organization/{organizationId}")]
    public async Task<IActionResult> GetByOrganizationId(int organizationId)
    {
        var query = new GetAllCropsByOrganizationIdQuery(organizationId);
        var crops = await _queryService.Handle(query);
        var resources = crops.Select(CropResourceFromEntityAssembler.ToResource);
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