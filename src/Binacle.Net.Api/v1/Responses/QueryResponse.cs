﻿using Binacle.Net.Api.Models;
using Binacle.Net.Api.v1.Models;
using Binacle.Net.Lib.Fitting.Models;

namespace Binacle.Net.Api.v1.Responses;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class QueryResponse : ResponseBase
{
	public Bin? Bin { get; set; }

	public static QueryResponse Create(FittingResult operationResult)
	{
		var response = new QueryResponse();

		if (operationResult.Status == Lib.Fitting.Models.FittingResultStatus.Success)
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
