using Binacle.Net.Kernel.OpenApi.Attributes;
using Binacle.Lib.Abstractions.Models;
using System.ComponentModel;

namespace Binacle.Net.v4.Contracts.Fit;

#pragma warning disable CS1591

[Description("The fit result for each bin considered.")]
[OpenApiRequireNonNullable]
public class FitCompareResponse
{
	[Description(SchemaDescriptions.Results)]
	public required List<FitBinResponse> Results { get; set; }

	// Walks the requested bins rather than the result dictionary so the response keeps the order
	// the caller sent the bins in.
	internal static FitCompareResponse From<TBin>(
		OperationParameters parameters,
		List<TBin> bins,
		IDictionary<string, OperationResult> operationResults
	)
		where TBin : class, IWithID
	{
		var results = new List<FitBinResponse>(bins.Count);
		foreach (var bin in bins)
		{
			if (!operationResults.TryGetValue(bin.ID, out var operationResult))
			{
				continue;
			}

			results.Add(FitBinResponse.From(parameters, operationResult));
		}

		return new FitCompareResponse
		{
			Results = results
		};
	}
}
