using Binacle.Net.Lib.Models;

namespace Binacle.Net.Api.Models.Responses;

public class QueryResponse : ResponseBase
{
    public Bin? Bin { get; set; }

	public static QueryResponse Create(BinFittingOperationResult operationResult)
	{
		var response = new QueryResponse();

		if (operationResult.Status == BinFitResultStatus.Success)
		{
			response.Bin = new Bin
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
