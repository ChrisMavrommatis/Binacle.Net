namespace Binacle.Net.v4.Contracts.Pack;

#pragma warning disable CS1591
public enum BinPackResultStatus
{
    Unknown = -1,
    FullyPacked = 0,
    PartiallyPacked = 1,    // PartiallyPacked (includes at least 1 items packed)
    NotPacked = 2
}