using System.Numerics;
using Binacle.Geometry;

namespace Binacle.ViPaq;

internal static class ProtocolReaderExtensions
{
	public static void ReadDimensions<TObject, T>(
		this ProtocolReader<T> protocolReader,
		ref TObject obj,
		BitSize bitSize
	)
		where T : struct, IBinaryInteger<T>
		where TObject : IWithDimensions<T>
	{
		switch (bitSize)
		{
			case BitSize.Eight:
				obj.Length = protocolReader.Read8Bits();
				obj.Width = protocolReader.Read8Bits();
				obj.Height = protocolReader.Read8Bits();
				break;
			case BitSize.Sixteen:
				obj.Length = protocolReader.Read16Bits();
				obj.Width = protocolReader.Read16Bits();
				obj.Height = protocolReader.Read16Bits();
				break;
			case BitSize.ThirtyTwo:
				obj.Length = protocolReader.Read32Bits();
				obj.Width = protocolReader.Read32Bits();
				obj.Height = protocolReader.Read32Bits();
				break;
			case BitSize.SixtyFour:
				obj.Length = EnsureWithinRange(protocolReader.Read64Bits());
				obj.Width = EnsureWithinRange(protocolReader.Read64Bits());
				obj.Height = EnsureWithinRange(protocolReader.Read64Bits());
				break;
			default:
				throw new ArgumentOutOfRangeException($"BitSize {bitSize} is not supported");
		}
	}

	public static void ReadCoordinates<TObject, T>(
		this ProtocolReader<T> protocolReader,
		ref TObject obj,
		BitSize bitSize
	)
		where T : struct, IBinaryInteger<T>
		where TObject : IWithCoordinates<T>
	{
		switch (bitSize)
		{
			case BitSize.Eight:
				obj.X = protocolReader.Read8Bits();
				obj.Y = protocolReader.Read8Bits();
				obj.Z = protocolReader.Read8Bits();
				break;
			case BitSize.Sixteen:
				obj.X = protocolReader.Read16Bits();
				obj.Y = protocolReader.Read16Bits();
				obj.Z = protocolReader.Read16Bits();
				break;
			case BitSize.ThirtyTwo:
				obj.X = protocolReader.Read32Bits();
				obj.Y = protocolReader.Read32Bits();
				obj.Z = protocolReader.Read32Bits();
				break;
			case BitSize.SixtyFour:
				obj.X = EnsureWithinRange(protocolReader.Read64Bits());
				obj.Y = EnsureWithinRange(protocolReader.Read64Bits());
				obj.Z = EnsureWithinRange(protocolReader.Read64Bits());
				break;
			default:
				throw new ArgumentOutOfRangeException($"BitSize {bitSize} is not supported");
		}
	}

	// Decode-side ceiling (PROTOCOL.md §5/§7): a 64-bit field on the wire can carry a value above
	// MaxInteger (2^53 - 1). Reject it instead of returning it — only SixtyFour fields can exceed it.
	private static T EnsureWithinRange<T>(T value)
		where T : struct, IBinaryInteger<T>
	{
		if (value > T.CreateSaturating(ViPaqLimits.MaxInteger))
		{
			throw new ArgumentOutOfRangeException(
				nameof(value),
				value,
				$"Decoded value exceeds the max supported value ({ViPaqLimits.MaxInteger})"
				);
		}
		return value;
	}
}
