using Binacle.Net.Api.Models;
using Binacle.Net.Api.Models.Responses;
using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Models;

namespace Binacle.Net.Api.Services;

public class LockerService : ILockerService
{
    private readonly Lib.StrategyFactory strategyFactory;

    public LockerService()
    {
        this.strategyFactory = new Lib.StrategyFactory();
    }

    public QueryResponse FindFittingBin<TBin, TBox>(List<TBin> bins, List<TBox> items)
        where TBin : class, IItemWithReadOnlyDimensions<int>
        where TBox : class, IItemWithReadOnlyDimensions<int>, IWithQuantity<int>
    {
        var strategy = this.strategyFactory.Create(Lib.BinFittingStrategy.DecreasingVolumeSize);

        var operation = strategy
            .WithBins(bins)
            .AndItems(items)
            .Build();

        var operationResult = operation.Execute();

        var response = new QueryResponse();

        if (operationResult.Status == BinFitResultStatus.Success)
        {
            response.Container = new Container
            {
                ID = operationResult.FoundBin.ID, 
                Height = operationResult.FoundBin.Height,
                Length = operationResult.FoundBin.Length,
                Width = operationResult.FoundBin.Width
            };
            response.Result = ResultType.Success;
        }
        else
        {
            response.Result = ResultType.Failure;
            response.Message = $"Failed to find bin. Reason: {operationResult.Reason.ToString()}";
        }
        
        return response;
    }
}
