using EcotrackPlatform.API.Organizations.Domain.Repositories;
using EcotrackPlatform.API.Shared.Domain.Repositories;

namespace EcotrackPlatform.API.Organizations.Application.Internal.CommandServices.Plots;

public enum DeletePlotError
{
    None,
    PlotNotFound
}

public record DeletePlotResult(DeletePlotError Error = DeletePlotError.None)
{
    public bool Success => Error == DeletePlotError.None;
}

public class DeletePlotCommandService(
    IPlotRepository plotRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<DeletePlotResult> DeleteAsync(int id)
    {
        var plot = await plotRepository.FindByIdAsync(id);
        if (plot == null) return new DeletePlotResult(Error: DeletePlotError.PlotNotFound);

        plotRepository.Remove(plot);
        await unitOfWork.CompleteAsync();

        return new DeletePlotResult();
    }
}