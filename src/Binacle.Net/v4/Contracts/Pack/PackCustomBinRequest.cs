namespace Binacle.Net.v4.Contracts.Pack;

#pragma warning disable CS1591
public class PackCustomBinRequest : IWithOperationParameters, IWithBin, IWithItems
{
    public required OperationParameters Parameters { get; set; }
    public required Bin Bin { get; set; } 
    public required List<Box> Items { get; set; }
}