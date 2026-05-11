﻿using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
 using EcotrackPlatform.API.Organizations.Domain.Model.Queries;

 namespace EcotrackPlatform.API.Organizations.Aplication.Services;

public interface IOrganizationQueryService
{
    Task<Organization?> Handle(GetOrganizationByIdQuery query);
    Task<IEnumerable<Organization>> Handle(GetAllOrganizationsQuery query);
    Task<IEnumerable<Organization>> HandleByMemberAsync(int profileId);
}