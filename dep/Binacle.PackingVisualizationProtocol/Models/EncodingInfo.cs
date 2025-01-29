namespace Binacle.PackingVisualizationProtocol.Models;

public struct EncodingInfo
{
	// [Version] [Byte Size] [IsFloating] [IsSigned] [IsLittleEndian] [FutureBit]
	// [2 Bits] [2 Bits] [1 Bit] [1 Bit] [1 Bit] [1 Bit]
	// 00010010 
	// 2 Bits
	public required Version Version { get; init; }
	
	// 2 bits
	public required ByteSize ByteSize { get; init; }  
	
	// 1 bit
	public bool IsFloating { get; init; }
	
	// 1 Bit
	public bool IsSigned { get; init; }
	
	// 1 Bit
	public bool IsLittleEndian { get; init; }
	
	// 1 Bit
	private bool FutureBit { get; init; }

	public static byte ToByte(EncodingInfo encodingInfo)
	{
		byte encodingInfoByte = 0;
		encodingInfoByte |= (byte)((byte)encodingInfo.Version << 6);
		encodingInfoByte |= (byte)((byte)encodingInfo.ByteSize << 4);
		encodingInfoByte |= (byte)(encodingInfo.IsFloating ? 0b0000_1000 : 0);
		encodingInfoByte |= (byte)(encodingInfo.IsSigned ? 0b0000_0100 : 0);
		encodingInfoByte |= (byte)(encodingInfo.IsLittleEndian ? 0b0000_0010 : 0);
		encodingInfoByte |= (byte)(encodingInfo.FutureBit ? 0b0000_0001 : 0);
		return encodingInfoByte;
	}
	
	public static EncodingInfo FromByte(byte encodingInfoByte)
	{
		var header = new EncodingInfo
		{
			Version = (Version)((encodingInfoByte & 0b1100_0000) >> 6),
			ByteSize = (ByteSize)((encodingInfoByte & 0b0011_0000) >> 4),
			IsFloating = (encodingInfoByte & 0b0000_1000) != 0,
			IsSigned = (encodingInfoByte & 0b0000_0100) != 0,
			IsLittleEndian = (encodingInfoByte & 0b0000_0010) != 0,
			FutureBit = (encodingInfoByte & 0b0000_0001) != 0
		};
		return header;
	}
}
