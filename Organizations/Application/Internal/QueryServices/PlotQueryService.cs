using EcotrackPlatform.API.Organizations.Application.Services;
using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Organizations.Domain.Model.Queries;
using EcotrackPlatform.API.Organizations.Domain.Repositories;

namespace EcotrackPlatform.API.Organizations.Application.Internal.QueryServices;

public class PlotQueryService(IPlotRepository plotRepository) : IPlotQueryService
{
    public async Task<Plot?> Handle(GetPlotByIdQuery query)
    {
        return await plotRepository.FindByIdAsync(query.Id);
    }

    public async Task<IEnumerable<Plot>> Handle(GetAllPlotsQuery query)
    {
        return await plotRepository.ListAsync();
    }

    public async Task<IEnumerable<Plot>> Handle(GetAllPlotsByOrganizationIdQuery query)
    {
        return await plotRepository.ListByOrganizationIdAsync(query.OrganizationId);
    }
}

