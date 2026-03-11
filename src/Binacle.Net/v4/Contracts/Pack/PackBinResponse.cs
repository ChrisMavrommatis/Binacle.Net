using System.Text.Json.Serialization;
using Binacle.Lib.Abstractions.Models;
using Binacle.Net.v4.ExtensionMethods;

namespace Binacle.Net.v4.Contracts.Pack;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class PackBinResponse : BinResponseBase
{
    [JsonPropertyOrder(0)]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BinPackResultStatus Status { get; set; }

    internal static PackBinResponse From(
        OperationParameters parameters,
        OperationResult operationResult
    )
    {
	    var result = From<PackBinResponse>(parameters, operationResult);
	    result.Status = operationResult.Status.MapToBinPackResultStatus();
        return result;
    }
}
