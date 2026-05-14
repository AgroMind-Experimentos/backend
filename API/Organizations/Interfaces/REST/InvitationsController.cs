using EcotrackPlatform.API.Organizations.Application.Internal.CommandServices;
using EcotrackPlatform.API.Organizations.Application.Services;
using EcotrackPlatform.API.Organizations.Domain.Model.Queries;
using EcotrackPlatform.API.Organizations.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;

namespace EcotrackPlatform.API.Organizations.Interfaces.REST;

[ApiController]
[Route("api/v1")]
public class InvitationsController(InvitationCommandService invitationService, IOrganizationQueryService orgQuery) : ControllerBase
{
    [HttpPost("organizations/{orgId:int}/invite")]
    public async Task<IActionResult> Invite([FromRoute] int orgId, [FromBody] InviteByEmailResource body)
    {
        var result = await invitationService.SendInvitationByEmailAsync(orgId, body.Email, body.AgronomistId);
        return result switch
        {
            InviteResult.Success        => Ok(new { message = "Invitation sent successfully." }),
            InviteResult.InvalidEmail   => BadRequest(new { message = "Invalid email format." }),
            InviteResult.UserNotFound   => NotFound(new { message = "No registered user found with that email address." }),
            InviteResult.DuplicatePending => Conflict(new { message = "This user already has a pending invitation to this organization." }),
            _                           => StatusCode(500, new { message = "Unexpected error." })
        };
    }

    [HttpGet("invitations")]
    public async Task<IActionResult> GetPending([FromQuery] int profileId)
    {
        var pending = await invitationService.GetPendingForFarmerAsync(profileId);

        var result = new List<object>();
        foreach (var inv in pending)
        {
            var org = await orgQuery.Handle(new GetOrganizationByIdQuery(inv.OrganizationId));
            result.Add(new
            {
                id = inv.Id,
                organizationId = inv.OrganizationId,
                organizationName = org?.Name ?? "Unknown",
                agronomistId = inv.AgronomistProfileId,
                createdAt = inv.CreatedAt
            });
        }
        return Ok(result);
    }

    [HttpPost("invitations/{id:int}/accept")]
    public async Task<IActionResult> Accept([FromRoute] int id, [FromBody] RespondInvitationResource body)
    {
        var ok = await invitationService.AcceptAsync(id, body.ProfileId);
        if (!ok) return BadRequest(new { message = "Cannot accept this invitation." });
        return Ok(new { message = "Invitation accepted. You are now a member." });
    }

    [HttpPost("invitations/{id:int}/reject")]
    public async Task<IActionResult> Reject([FromRoute] int id, [FromBody] RespondInvitationResource body)
    {
        var ok = await invitationService.RejectAsync(id, body.ProfileId);
        if (!ok) return BadRequest(new { message = "Cannot reject this invitation." });
        return Ok(new { message = "Invitation rejected." });
    }
}
