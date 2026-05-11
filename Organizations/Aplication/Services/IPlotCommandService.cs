using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Organizations.Domain.Model.Commands;

namespace EcotrackPlatform.API.Organizations.Aplication.Services;

public interface IPlotCommandService
{
    Task<Plot> Handle(CreatePlotCommand command);
    Task<Plot?> UpdateAsync(int id, string? name, string? location, double? area, string? cultivation, List<int>? memberIds);
    Task<bool> Handle(int id);
}