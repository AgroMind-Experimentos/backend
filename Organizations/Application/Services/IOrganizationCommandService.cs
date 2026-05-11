using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Organizations.Domain.Model.Commands;

namespace EcotrackPlatform.API.Organizations.Application.Services;

public interface IOrganizationCommandService
{
    Task<Organization> Handle(CreateOrganizationCommand command);
    Task<Organization?> UpdateAsync(UpdateOrganizationCommand command);
    Task<bool> Handle(DeleteOrganizationByIdCommand command);
}