using Binacle.Net.Kernel.OpenApi.Attributes;
using Binacle.Lib.Abstractions.Models;
using System.ComponentModel;

namespace Binacle.Net.v4.Contracts.Pack;

#pragma warning disable CS1591

[Description("The packing result for each bin considered.")]
[OpenApiRequireNonNullable]
public class PackCompareResponse
{
	[Description(SchemaDescriptions.Results)]
	public required List<PackBinResponse> Results { get; set; }

	internal static PackCompareResponse From<TBin>(
		OperationParameters parameters,
		List<TBin> bins,
		IDictionary<string, OperationResult> operationResults
	)
		where TBin : class, IWithID
	{
		var results = new List<PackBinResponse>(bins.Count);
		foreach (var bin in bins)
		{
			if (!operationResults.TryGetValue(bin.ID, out var operationResult))
			{
				continue;
			}

			results.Add(PackBinResponse.From(parameters, operationResult));
		}

		return new PackCompareResponse
		{
			Results = results
		};
	}
}
