﻿using EcotrackPlatform.API.Organization.Domain.Model.Commands;

namespace EcotrackPlatform.API.Organization.Aplication.Services;

public interface IOrganizationCommandService
{
    Task<Domain.Model.Aggregates.Organization> Handle(CreateOrganizationCommand command);
    Task<Domain.Model.Aggregates.Organization?> UpdateAsync(int id, string? name, string? description, string? status, List<int>? memberIds);
    Task<bool> Handle(int id);
}