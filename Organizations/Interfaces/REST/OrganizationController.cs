using EcotrackPlatform.API.Organizations.Application.Internal.CommandServices.Organizations;
using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Organizations.Domain.Model.Queries;
using EcotrackPlatform.API.Organizations.Application.Services;
using EcotrackPlatform.API.Organizations.Domain.Model.Commands;
using EcotrackPlatform.API.Organizations.Interfaces.REST.Resources;
using EcotrackPlatform.API.Organizations.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;

namespace EcotrackPlatform.API.Organizations.Interfaces.REST;

[ApiController]
[Route("api/v1/organizations")]
public class OrganizationsController(
    CreateOrganizationCommandService createService,
    UpdateOrganizationCommandService updateService,
    DeleteOrganizationCommandService deleteService,
    IOrganizationQueryService queryService)
    : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrganizationResource resource)
    {
        var command = CreateOrganizationCommandFromResourceAssembler.ToCommand(resource);
        var result = await createService.CreateAsync(command);

        if (result.Success)
        {
            var resourceResult = OrganizationResourceFromEntityAssembler.ToResource(result.Organization!);
            return CreatedAtAction(nameof(GetById), new { id = resourceResult.Id }, resourceResult);
        }

        return result.Error switch
        {
            CreateOrganizationError.InvalidOrganizationData => BadRequest(new { message = "invalidOrganizationData" }),
            _ => StatusCode(500)
        };
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateOrganizationResource resource)
    {
        var command = UpdateOrganizationCommandFromResourceAssembler.ToCommand(id, resource);
        var result = await updateService.UpdateAsync(command);

        if (result.Success)
        {
            return Ok(OrganizationResourceFromEntityAssembler.ToResource(result.Organization!));
        }

        return result.Error switch
        {
            UpdateOrganizationError.OrganizationNotFound => NotFound(new { message = "organizationNotFound" }),
            UpdateOrganizationError.ProfileNotFound => NotFound(new { message = "profileNotFound" }),
            UpdateOrganizationError.InvalidOrganizationData => BadRequest(new { message = "invalidOrganizationData" }),
            _ => StatusCode(500)
        };
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? profileId)
    {
        IEnumerable<Organization> organizations;

        if (profileId.HasValue)
            organizations = await queryService.HandleByMemberAsync(new GetOrganizationByMemberIdQuery((int) profileId));
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
        var command = new DeleteOrganizationByIdCommand(id);
        var result = await deleteService.DeleteAsync(command);

        if (result.Success) return NoContent();

        return result.Error switch
        {
            DeleteOrganizationError.OrganizationNotFound => NotFound(new { message = "organizationNotFound" }),
            _ => StatusCode(500)
        };
    }
}