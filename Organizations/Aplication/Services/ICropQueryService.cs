using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Organizations.Domain.Model.Queries;

namespace EcotrackPlatform.API.Organizations.Aplication.Services;

public interface ICropQueryService
{
    Task<Crop?> Handle(GetCropByIdQuery query);
    Task<IEnumerable<Crop>> Handle(GetAllCropsQuery query);
    Task<IEnumerable<Crop>> Handle(GetAllCropsByOrganizationIdQuery query);
}