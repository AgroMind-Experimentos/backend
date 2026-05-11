using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Organizations.Domain.Model.Queries;

namespace EcotrackPlatform.API.Organizations.Application.Services;

public interface IPlotQueryService
{
    Task<Plot?> Handle(GetPlotByIdQuery query);
    Task<IEnumerable<Plot>> Handle(GetAllPlotsQuery query);
    Task<IEnumerable<Plot>> Handle(GetAllPlotsByOrganizationIdQuery query);
}