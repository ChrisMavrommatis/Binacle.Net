using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Api.v1.Responses;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class QueryResponse : v1.Models.ResponseBase
{
	public v1.Models.Bin? Bin { get; set; }

	public static QueryResponse Create<TBin, TItem>(
		List<TBin> bins, 
		List<TItem> items, 
		IDictionary<string, Lib.Fitting.Models.FittingResult> operationResults
	)
		where  TBin : IWithID, IWithReadOnlyDimensions
		where  TItem : IWithID, IWithReadOnlyDimensions
	{
		if (operationResults.Count > 1)
		{
			throw new InvalidOperationException("Expected at most one fitting operation result at v1 api");
		}
		var response = new QueryResponse();

		if (operationResults.Count == 0)
		{
			response.Result = v1.Models.ResultType.Failure;
			response.Message = $"Failed to find bin.";
			//response.Message = $"Failed to find bin. Reason: {operationResult.Reason.ToString()}";
		}
		else
		{
			var (binId, operationResult) = operationResults.First();
			var bin = bins.First(x => x.ID == binId);

			response.Bin = new v1.Models.Bin
			{
				ID = bin.ID,
				Height = bin.Height,
				Length = bin.Length,
				Width = bin.Width
			};
			response.Result = v1.Models.ResultType.Success;
		}

		return response;
	}
}
