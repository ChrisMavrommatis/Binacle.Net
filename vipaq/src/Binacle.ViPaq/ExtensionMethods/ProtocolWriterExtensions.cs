using System.Numerics;
using Binacle.Geometry;

namespace Binacle.ViPaq;

internal static class ProtocolWriterExtensions
{
	public static void WriteDimensions<TObject, T>(
		this ProtocolWriter<T> protocolWriter,
		TObject obj,
		BitSize bitSize
	)
		where T : struct, IBinaryInteger<T>
		where TObject : IWithDimensions<T>
	{
		switch (bitSize)
		{
			case BitSize.Eight:
				protocolWriter.Write8Bits(obj.Length);
				protocolWriter.Write8Bits(obj.Width);
				protocolWriter.Write8Bits(obj.Height);
				break;
			case BitSize.Sixteen:
				protocolWriter.Write16Bits(obj.Length);
				protocolWriter.Write16Bits(obj.Width);
				protocolWriter.Write16Bits(obj.Height);
				break;
			case BitSize.ThirtyTwo:
				protocolWriter.Write32Bits(obj.Length);
				protocolWriter.Write32Bits(obj.Width);
				protocolWriter.Write32Bits(obj.Height);
				break;
			case BitSize.SixtyFour:
				protocolWriter.Write64Bits(obj.Length);
				protocolWriter.Write64Bits(obj.Width);
				protocolWriter.Write64Bits(obj.Height);
				break;
			default:
				throw new ArgumentOutOfRangeException($"BitSize {bitSize} is not supported");
		}
	}

	public static void WriteCoordinates<TObject, T>(
		this ProtocolWriter<T> protocolWriter,
		TObject obj,
		BitSize bitSize
	)
		where T : struct, IBinaryInteger<T>
		where TObject : IWithCoordinates<T>
	{
		switch (bitSize)
		{
			case BitSize.Eight:
				protocolWriter.Write8Bits(obj.X);
				protocolWriter.Write8Bits(obj.Y);
				protocolWriter.Write8Bits(obj.Z);
				break;
			case BitSize.Sixteen:
				protocolWriter.Write16Bits(obj.X);
				protocolWriter.Write16Bits(obj.Y);
				protocolWriter.Write16Bits(obj.Z);
				break;
			case BitSize.ThirtyTwo:
				protocolWriter.Write32Bits(obj.X);
				protocolWriter.Write32Bits(obj.Y);
				protocolWriter.Write32Bits(obj.Z);
				break;
			case BitSize.SixtyFour:
				protocolWriter.Write64Bits(obj.X);
				protocolWriter.Write64Bits(obj.Y);
				protocolWriter.Write64Bits(obj.Z);
				break;
			default:
				throw new ArgumentOutOfRangeException($"BitSize {bitSize} is not supported");
		}
	}
}
