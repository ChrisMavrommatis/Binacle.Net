using System.Text.Json.Serialization;
using Binacle.Lib.Abstractions.Models;
using Binacle.Net.v4.ExtensionMethods;

namespace Binacle.Net.v4.Contracts.Fit;

#pragma warning disable CS1591

public class FitBinResponse : BinResponseBase
{
	[JsonPropertyOrder(0)]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public BinFitResultStatus Status { get; set; }

	[JsonPropertyOrder(1)]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public BinFitEarlyExitReason EarlyExitReason { get; set; }

	internal static FitBinResponse From(
		OperationParameters parameters,
		OperationResult operationResult
	)
	{
		var result = From<FitBinResponse>(parameters, operationResult);
		result.Status = operationResult.Status.MapToBinFitResultStatus();
		result.EarlyExitReason = operationResult.EarlyExitReason.MapToBinFitEarlyExitReason();
		return result;
	}
}
