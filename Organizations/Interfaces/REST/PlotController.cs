using EcotrackPlatform.API.Organizations.Application.Internal.CommandServices.Plots;
using EcotrackPlatform.API.Organizations.Application.Services;
using EcotrackPlatform.API.Organizations.Domain.Model.Queries;
using EcotrackPlatform.API.Organizations.Interfaces.REST.Resources;
using EcotrackPlatform.API.Organizations.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;

namespace EcotrackPlatform.API.Organizations.Interfaces.REST;

[ApiController]
[Route("api/v1/plots")]
public class PlotsController(
    CreatePlotCommandService createService,
    UpdatePlotCommandService updateService,
    DeletePlotCommandService deleteService,
    IPlotQueryService queryService)
    : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePlotResource resource)
    {
        var command = CreatePlotCommandFromResourceAssembler.ToCommand(resource);
        var result = await createService.CreateAsync(command);

        if (result.Success)
        {
            var resourceResult = PlotResourceFromEntityAssembler.ToResource(result.Plot!);
            return CreatedAtAction(nameof(GetById), new { id = resourceResult.Id }, resourceResult);
        }

        return result.Error switch
        {
            CreatePlotError.OrganizationNotFound => NotFound(new { message = "organizationNotFound" }),
            CreatePlotError.InvalidPlotData => BadRequest(new { message = "invalidPlotData" }),
            _ => StatusCode(500)
        };
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdatePlotResource resource)
    {
        var result = await updateService.UpdateAsync(UpdatePlotCommandFromResourceAssembler.ToCommand(id, resource));

        if (result.Success)
        {
            return Ok(PlotResourceFromEntityAssembler.ToResource(result.Plot!));
        }

        return result.Error switch
        {
            UpdatePlotError.PlotNotFound => NotFound(new { message = "plotNotFound" }),
            UpdatePlotError.OrganizationNotFound => NotFound(new { message = "organizationNotFound" }),
            UpdatePlotError.ProfileNotFound => NotFound(new { message = "profileNotFound" }),
            UpdatePlotError.ProfileNotInOrganization => BadRequest(new { message = "profileNotInOrganization" }),
            UpdatePlotError.InvalidPlotData => BadRequest(new { message = "invalidPlotData" }),
            _ => StatusCode(500)
        };
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetAllPlotsQuery();
        var plots = await queryService.Handle(query);
        var resources = plots.Select(PlotResourceFromEntityAssembler.ToResource);
        return Ok(resources);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GetPlotByIdQuery(id);
        var plot = await queryService.Handle(query);

        if (plot == null)
            return NotFound();

        var resource = PlotResourceFromEntityAssembler.ToResource(plot);
        return Ok(resource);
    }

    [HttpGet("organization/{organizationId}")]
    public async Task<IActionResult> GetByOrganizationId(int organizationId)
    {
        var query = new GetAllPlotsByOrganizationIdQuery(organizationId);
        var plots = await queryService.Handle(query);
        var resources = plots.Select(PlotResourceFromEntityAssembler.ToResource);
        return Ok(resources);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await deleteService.DeleteAsync(id);

        if (result.Success) return NoContent();

        return result.Error switch
        {
            DeletePlotError.PlotNotFound => NotFound(new { message = "plotNotFound" }),
            _ => StatusCode(500)
        };
    }
}