using System.Numerics;
using Binacle.ViPaq.Abstractions;

namespace Binacle.ViPaq;

public static class ProtocolReaderExtensions
{
	public static void ReadDimensions<TObject, T>(
		this ProtocolReader<T> protocolReader,
		ref TObject obj,
		BitSize bitSize
	)
		where T : struct, IBinaryInteger<T>, INumber<T>, IComparable<T>
		where TObject : IWithDimensions<T>
	{
		switch (bitSize)
		{
			case BitSize.Eight:
				obj.Length = protocolReader.ReadAsByte();
				obj.Width = protocolReader.ReadAsByte();
				obj.Height = protocolReader.ReadAsByte();
				break;
			case BitSize.Sixteen:
				obj.Length = protocolReader.ReadAsUInt16();
				obj.Width = protocolReader.ReadAsUInt16();
				obj.Height = protocolReader.ReadAsUInt16();
				break;
			case BitSize.ThirtyTwo:
				obj.Length = protocolReader.ReadAsUInt32();
				obj.Width = protocolReader.ReadAsUInt32();
				obj.Height = protocolReader.ReadAsUInt32();
				break;
			case BitSize.SixtyFour:
				obj.Length = protocolReader.ReadAsUInt64();
				obj.Width = protocolReader.ReadAsUInt64();
				obj.Height = protocolReader.ReadAsUInt64();
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
		where T : struct, IBinaryInteger<T>, INumber<T>, IComparable<T>
		where TObject : IWithCoordinates<T>
	{
		switch (bitSize)
		{
			case BitSize.Eight:
				obj.X = protocolReader.ReadAsByte();
				obj.Y = protocolReader.ReadAsByte();
				obj.Z = protocolReader.ReadAsByte();
				break;
			case BitSize.Sixteen:
				obj.X = protocolReader.ReadAsUInt16();
				obj.Y = protocolReader.ReadAsUInt16();
				obj.Z = protocolReader.ReadAsUInt16();
				break;
			case BitSize.ThirtyTwo:
				obj.X = protocolReader.ReadAsUInt32();
				obj.Y = protocolReader.ReadAsUInt32();
				obj.Z = protocolReader.ReadAsUInt32();
				break;
			case BitSize.SixtyFour:
				obj.X = protocolReader.ReadAsUInt64();
				obj.Y = protocolReader.ReadAsUInt64();
				obj.Z = protocolReader.ReadAsUInt64();
				break;
			default:
				throw new ArgumentOutOfRangeException($"BitSize {bitSize} is not supported");
		}
	}
}
