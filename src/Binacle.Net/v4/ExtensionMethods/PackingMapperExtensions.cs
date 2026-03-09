using Binacle.Lib.Abstractions.Models;
using Binacle.Net.ExtensionMethods;
using Binacle.Net.v4.Contracts.Pack;

namespace Binacle.Net.v4.ExtensionMethods;

internal static class PackingMapperExtensions
{
    public static BinPackResultStatus MapToBinPackResultStatus(this OperationResultStatus operationResultStatus)
    {
        return operationResultStatus switch
        {
            OperationResultStatus.FullyPacked => BinPackResultStatus.FullyPacked,
            OperationResultStatus.PartiallyPacked => BinPackResultStatus.PartiallyPacked,
            OperationResultStatus.Unknown => BinPackResultStatus.Unknown,
            OperationResultStatus.NotPacked => BinPackResultStatus.NotPacked,
            _ => throw new NotSupportedException($"No Implementation exists for operation result  status {operationResultStatus.ToFastString()}"),
        };
    }
}
