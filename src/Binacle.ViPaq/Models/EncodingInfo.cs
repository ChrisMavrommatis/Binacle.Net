namespace Binacle.ViPaq;

public struct EncodingInfo
{
	// [Version] [Bin Dimensions Bit Size] [Item Dimensions Bit Size] [Item Coordinates Bit Size]
	// [2 Bits]  [2 Bits]                  [2 Bits]                  [2 Bits]

	// 2 Bits
	public required Version Version { get; set; }

	// 2 bits
	public required BitSize BinDimensionsBitSize { get; init; }

	// 2 bits
	public required BitSize ItemDimensionsBitSize { get; init; }

	// 2 bits
	public required BitSize ItemCoordinatesBitSize { get; init; }

}

