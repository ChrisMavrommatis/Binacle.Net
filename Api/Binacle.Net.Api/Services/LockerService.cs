using Binacle.Net.Api.Models;
using Binacle.Net.Api.Responses;
using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Models;

namespace Binacle.Net.Api.Services
{
    public class LockerService : ILockerService
    {
        private readonly Lib.StrategyFactory strategyFactory;

        public LockerService()
        {
            this.strategyFactory = new Lib.StrategyFactory();
        }

        public QueryResponse FindFittingBin<TBin, TBox>(List<TBin> bins, List<TBox> items)
            where TBin : class, IWithID, IWithReadOnlyDimensions<int>
            where TBox : class, IWithID, IWithReadOnlyDimensions<int>, IWithQuantity<int>
        {
            var strategy = this.strategyFactory.Create(Lib.Strategies.BinFittingStrategy.DecreasingVolumeSizeFirstFittingOrientation);

            var operation = strategy
                .WithBins(bins)
                .AndItems(items)
                .Build();

            var operationResult = operation.Execute();

            var response = new QueryResponse();

            if (operationResult.Status == BinFitResultStatus.Success)
            {
                response.Container = new Container(operationResult.FoundBin.ID, operationResult.FoundBin);
                response.Result = ApiResponseResultType.Success;
            }
            else
            {
                response.Result = ApiResponseResultType.Error;
                response.Message = $"Failed to find bin. Reason: {operationResult.Reason.ToString()}";
            }
            
            return response;
        }


       
    }
}
