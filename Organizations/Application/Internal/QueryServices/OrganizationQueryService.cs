using EcotrackPlatform.API.Organizations.Application.Services;
using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Organizations.Domain.Model.Queries;
using EcotrackPlatform.API.Organizations.Domain.Repositories;

namespace EcotrackPlatform.API.Organizations.Application.Internal.QueryServices;

public class OrganizationQueryService(IOrganizationRepository organizationRepository) : IOrganizationQueryService
{
    public async Task<Organization?> Handle(GetOrganizationByIdQuery query)
    {
        return await organizationRepository.FindByIdWithMembersAsync(query.Id);
    }

    public async Task<IEnumerable<Organization>> Handle(GetAllOrganizationsQuery query)
    {
        return await organizationRepository.ListWithMembersAsync();
    }

    public async Task<IEnumerable<Organization>> HandleByMemberAsync(GetOrganizationByMemberIdQuery query)
    {
        return await organizationRepository.ListByMemberAsync(query.Id);
    }
}

