using System.Text.Json.Serialization;
using Binacle.Lib.Abstractions.Models;

namespace Binacle.Net.v4.Contracts.Pack;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class BinPackResponse
{
    internal static BinPackResponse Create<TBin, TItem>(
        TBin bin,
        List<TItem> items,
        OperationParameters parameters,
        OperationResult operationResult
    )
        where TBin : class, IWithID, IWithReadOnlyDimensions
        where TItem : class, IWithID, IWithReadOnlyDimensions
    {
        return new BinPackResponse()
        {

        };
    }
}


public class BinPackResult
{
    [JsonPropertyOrder(0)]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required OperationResultStatus Status { get; set; }
    public required Bin Bin { get; set; }
    public List<PackedBox>? PackedItems { get; set; }
    public List<UnpackedBox>? UnpackedItems { get; set; }
    public required decimal PackedItemsVolumePercentage { get; set; }
    public required decimal PackedBinVolumePercentage { get; set; }
    public string? ViPaqData { get; set; }
}
