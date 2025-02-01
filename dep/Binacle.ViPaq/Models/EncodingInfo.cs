namespace Binacle.ViPaq.Models;

internal struct EncodingInfo
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


	public static byte ToByte(EncodingInfo encodingInfo)
	{
		byte encodingInfoByte = 0;
		encodingInfoByte |= (byte)((byte)encodingInfo.Version << 6);
		encodingInfoByte |= (byte)((byte)encodingInfo.BinDimensionsBitSize << 4);
		encodingInfoByte |= (byte)((byte)encodingInfo.ItemDimensionsBitSize << 2);
		encodingInfoByte |= (byte)((byte)encodingInfo.ItemCoordinatesBitSize);
		return encodingInfoByte;
	}

	public static EncodingInfo FromByte(byte firstByte)
	{
		return new EncodingInfo
		{
			Version = (Version)((firstByte & 0b11000000) >> 6),
			BinDimensionsBitSize = (BitSize)((firstByte & 0b00110000) >> 4),
			ItemDimensionsBitSize = (BitSize)((firstByte & 0b00001100) >> 2),
			ItemCoordinatesBitSize = (BitSize)(firstByte & 0b00000011)
		};
	}
}

