using EcotrackPlatform.API.Organizations.Domain.Model.Commands;
using EcotrackPlatform.API.Organizations.Domain.Repositories;
using EcotrackPlatform.API.Shared.Domain.Repositories;

namespace EcotrackPlatform.API.Organizations.Application.Internal.CommandServices.Organizations;

public enum DeleteOrganizationError
{
    None,
    OrganizationNotFound
}

public record DeleteOrganizationResult(DeleteOrganizationError Error = DeleteOrganizationError.None)
{
    public bool Success => Error == DeleteOrganizationError.None;
}

public class DeleteOrganizationCommandService(
    IOrganizationRepository repository,
    IInvitationRepository invitationRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<DeleteOrganizationResult> DeleteAsync(DeleteOrganizationByIdCommand command)
    {
        var organization = await repository.FindByIdAsync(command.Id);
        if (organization is null) return new DeleteOrganizationResult(Error: DeleteOrganizationError.OrganizationNotFound);

        var pendingInvitations = await invitationRepository.FindPendingByOrganizationAsync(command.Id);
        foreach (var invitation in pendingInvitations)
        {
            invitation.Cancel();
            invitationRepository.Update(invitation);
        }

        repository.Remove(organization);
        await unitOfWork.CompleteAsync();
        return new DeleteOrganizationResult();
    }
}