using System.Text.Json.Serialization;
using Binacle.Lib.Abstractions.Models;
using Binacle.Net.ExtensionMethods;
using Binacle.Net.v4.ExtensionMethods;
using Binacle.ViPaq;

namespace Binacle.Net.v4.Contracts.Pack;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class PackBinResponse
{
    [JsonPropertyOrder(0)]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required BinPackResultStatus Status { get; set; }

    public required Bin Bin { get; set; }
    public required string AlgorithmUsed { get; set; }
    public List<PackedBox>? PackedItems { get; set; }
    public List<UnpackedBox>? UnpackedItems { get; set; }
    public required decimal PackedItemsVolumePercentage { get; set; }
    public required decimal PackedBinVolumePercentage { get; set; }
    public string? ViPaqData { get; set; }


    internal static PackBinResponse From(
        OperationParameters parameters,
        OperationResult operationResult
    )
    {
        var result = new PackBinResponse()
        {
            Status = operationResult.Status.MapToBinPackResultStatus(),
            Bin = Bin.From(operationResult.Bin),
            AlgorithmUsed = operationResult.AlgorithmInfo.Algorithm.ToFastString(),
            PackedItems = operationResult.PackedItems
                .Select(x => PackedBox.From(x))
                .ToList(),
            UnpackedItems = operationResult.UnpackedItems
                .Select(x => UnpackedBox.From(x))
                .ToList(),
            PackedBinVolumePercentage = operationResult.PackedBinVolumePercentage,
            PackedItemsVolumePercentage = operationResult.PackedItemsVolumePercentage,
        };
        if (parameters.IncludeViPaqData)
        {
            if (result.PackedItems is not null && result.PackedItems.Count > 0)
            {
                var serializedResult = ViPaqSerializer.SerializeInt32(result.Bin, result.PackedItems!);
                result.ViPaqData = Convert.ToBase64String(serializedResult);
            }		
        }
        return result;
    }
}
