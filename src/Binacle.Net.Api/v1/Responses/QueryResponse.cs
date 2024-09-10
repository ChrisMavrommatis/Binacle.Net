using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Api.v1.Responses;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class QueryResponse : v1.Models.ResponseBase
{
	public v1.Models.Bin? Bin { get; set; }

	public static QueryResponse Create<TBin, TItem>(
		List<TBin> bins, 
		List<TItem> items, 
		Dictionary<string, Lib.Fitting.Models.FittingResult> operationResults
	)
		where  TBin : IWithID, IWithReadOnlyDimensions
		where  TItem : IWithID, IWithReadOnlyDimensions
	{
		if (operationResults.Count != 1)
		{
			throw new InvalidOperationException("Expected exactly one fitting operation result at v1 api");
		}
		var (binId, operationResult) = operationResults.First();
		var bin = bins.First(x => x.ID == binId);

		var response = new QueryResponse();

		if (operationResult.Status == Lib.Fitting.Models.FittingResultStatus.Success)
		{
			response.Bin = new v1.Models.Bin
			{
				ID = bin.ID,
				Height = bin.Height,
				Length = bin.Length,
				Width = bin.Width
			};
			response.Result = v1.Models.ResultType.Success;
		}
		else
		{
			response.Result = v1.Models.ResultType.Failure;
			response.Message = $"Failed to find bin. Reason: {operationResult.Reason.ToString()}";
		}

		return response;
	}
}
