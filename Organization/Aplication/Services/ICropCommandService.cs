﻿using EcotrackPlatform.API.Organization.Domain.Model.Aggregates;
using EcotrackPlatform.API.Organization.Domain.Model.Commands;

namespace EcotrackPlatform.API.Organization.Aplication.Services;

public interface ICropCommandService
{
    Task<Crop> Handle(CreateCropCommand command);
    Task<Crop?> UpdateAsync(int id, string? name, string? location, double? area, string? cultivation, List<int>? memberIds);
    Task<bool> Handle(int id);
}