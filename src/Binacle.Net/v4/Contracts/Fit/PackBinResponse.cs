using System.Text.Json.Serialization;
using Binacle.Lib.Abstractions.Models;
using Binacle.Net.ExtensionMethods;
using Binacle.Net.v4.ExtensionMethods;
using Binacle.ViPaq;

namespace Binacle.Net.v4.Contracts.Fit;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public enum BinFitResultStatus
{
	Unknown = -1,
	Fits = 0,
	DoesNotFit = 1,
	EarlyExit = 2,
}

public enum BinFitEarlyExitReason
{
	Unknown = -1,
	Fits = 0,
	DoesNotFit = 1,
	EarlyExit = 2,
}
	

public class FitBinResponse
{
    [JsonPropertyOrder(0)]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required BinFitResultStatus Status { get; set; }

    public required Bin Bin { get; set; }
    public required string AlgorithmUsed { get; set; }
    public List<PackedBox>? FittedItems { get; set; }
    public List<UnpackedBox>? UnfittedItems { get; set; }
    public required decimal FittedItemsVolumePercentage { get; set; }
    public required decimal FittedBinVolumePercentage { get; set; }
    public string? ViPaqData { get; set; }


    internal static FitBinResponse From(
        OperationParameters parameters,
        OperationResult operationResult
    )
    {
        var result = new FitBinResponse()
        {
            Status = operationResult.Status.MapToBinFitResultStatus(),
            Bin = Bin.From(operationResult.Bin),
            AlgorithmUsed = operationResult.AlgorithmInfo.Algorithm.ToFastString(),
            FittedItems = operationResult.PackedItems
                .Select(x => PackedBox.From(x))
                .ToList(),
            UnfittedItems = operationResult.UnpackedItems
                .Select(x => UnpackedBox.From(x))
                .ToList(),
            FittedItemsVolumePercentage = operationResult.PackedBinVolumePercentage,
            FittedBinVolumePercentage = operationResult.PackedItemsVolumePercentage,
        };
        if (parameters.IncludeViPaqData)
        {
            if (result.FittedItems is not null && result.FittedItems.Count > 0)
            {
                var serializedResult = ViPaqSerializer.SerializeInt32(result.Bin, result.FittedItems!);
                result.ViPaqData = Convert.ToBase64String(serializedResult);
            }		
        }
        return result;
    }
}
