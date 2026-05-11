using EcotrackPlatform.API.Organizations.Aplication.Services;
using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Organizations.Domain.Model.Commands;
using EcotrackPlatform.API.Organizations.Domain.Model.Queries;
using EcotrackPlatform.API.Organizations.Interfaces.REST.Resources;
using EcotrackPlatform.API.Organizations.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;

namespace EcotrackPlatform.API.Organizations.Interfaces.REST;

[ApiController]
[Route("api/v1/organizations")]
public class OrganizationsController(IOrganizationCommandService commandService, IOrganizationQueryService queryService)
    : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrganizationResource resource)
    {
        try
        {
            var command = CreateOrganizationCommandFromResourceAssembler.ToCommand(resource);
            var result = await commandService.Handle(command);
            var resourceResult = OrganizationResourceFromEntityAssembler.ToResource(result);
            return CreatedAtAction(nameof(GetById), new { id = resourceResult.Id }, resourceResult);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateOrganizationResource resource)
    {
        try
        {
            var updated = await commandService.UpdateAsync(UpdateOrganizationCommandFromResourceAssembler.ToCommand(id, resource));
            if (updated is null) return NotFound();

            return Ok(OrganizationResourceFromEntityAssembler.ToResource(updated));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? profileId)
    {
        IEnumerable<Organization> organizations;

        if (profileId.HasValue)
            organizations = await queryService.HandleByMemberAsync(profileId.Value);
        else
        {
            var query = new GetAllOrganizationsQuery();
            organizations = await queryService.Handle(query);
        }

        var resources = organizations.Select(OrganizationResourceFromEntityAssembler.ToResource);
        return Ok(resources);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var query = new GetOrganizationByIdQuery(id);
        var organization = await queryService.Handle(query);
        
        if (organization == null)
            return NotFound();
        
        var resource = OrganizationResourceFromEntityAssembler.ToResource(organization);
        return Ok(resource);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var deleted = await commandService.Handle(new DeleteOrganizationByIdCommand(id));
        if (!deleted) return NotFound();
        return NoContent();
    }
}