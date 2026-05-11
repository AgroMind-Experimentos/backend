﻿using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
 using EcotrackPlatform.API.Organizations.Domain.Model.Commands;

 namespace EcotrackPlatform.API.Organizations.Aplication.Services;

public interface IOrganizationCommandService
{
    Task<Organization> Handle(CreateOrganizationCommand command);
    Task<Organization?> UpdateAsync(int id, string? name, string? description, string? status, List<int>? memberIds);
    Task<bool> Handle(int id);
}