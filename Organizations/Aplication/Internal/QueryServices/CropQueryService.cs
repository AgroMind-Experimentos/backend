using EcotrackPlatform.API.Organizations.Aplication.Services;
using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Organizations.Domain.Model.Queries;
using EcotrackPlatform.API.Organizations.Domain.Repositories;

namespace EcotrackPlatform.API.Organizations.Aplication.Internal.QueryServices;

public class CropQueryService(ICropRepository cropRepository) : ICropQueryService
{
    public async Task<Crop?> Handle(GetCropByIdQuery query)
    {
        return await cropRepository.FindByIdWithMembersAsync(query.Id);
    }

    public async Task<IEnumerable<Crop>> Handle(GetAllCropsQuery query)
    {
        return await cropRepository.ListWithMembersAsync();
    }

    public async Task<IEnumerable<Crop>> Handle(GetAllCropsByOrganizationIdQuery query)
    {
        return await cropRepository.FindByOrganizationIdWithMembersAsync(query.OrganizationId);
    }
}

