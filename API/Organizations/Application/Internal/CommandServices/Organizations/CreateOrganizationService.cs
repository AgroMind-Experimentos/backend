using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Organizations.Domain.Model.Commands;
using EcotrackPlatform.API.Organizations.Domain.Repositories;
using EcotrackPlatform.API.Shared.Domain.Repositories;

namespace EcotrackPlatform.API.Organizations.Application.Internal.CommandServices.Organizations;

public enum CreateOrganizationError
{
    None,
    InvalidOrganizationData
}

public record CreateOrganizationResult(Organization? Organization = null, CreateOrganizationError Error = CreateOrganizationError.None)
{
    public bool Success => Error == CreateOrganizationError.None;
}

public class CreateOrganizationCommandService(
    IOrganizationRepository repository,
    IUnitOfWork unitOfWork)
{
    public async Task<CreateOrganizationResult> CreateAsync(CreateOrganizationCommand command)
    {
        try
        {
            var org = new Organization(
                command.Name,
                command.Description,
                command.Location,
                command.AgronomistId
            );

            await repository.AddAsync(org);
            await unitOfWork.CompleteAsync();

            org.SyncMembers([command.AgronomistId]);
            repository.Update(org);
            await unitOfWork.CompleteAsync();

            return new CreateOrganizationResult(Organization: org);
        }
        catch (Exception)
        {
            return new CreateOrganizationResult(Error: CreateOrganizationError.InvalidOrganizationData);
        }
    }
}