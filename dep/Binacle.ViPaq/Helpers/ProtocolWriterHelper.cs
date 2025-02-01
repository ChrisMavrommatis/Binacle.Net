using System.Numerics;
using Binacle.ViPaq.Abstractions;
using Binacle.ViPaq.Models;

namespace Binacle.ViPaq.Helpers;

internal static class ProtocolWriterHelper
{
	public static void WriteDimensions<TObject, T>(
		this ProtocolWriter<T> protocolWriter,
		TObject obj,
		BitSize bitSize
	)
		where T : struct, IBinaryInteger<T>, INumber<T>, IComparable<T>
		where TObject : IWithDimensions<T>
	{
		switch (bitSize)
		{
			case BitSize.Eight:
				protocolWriter.WriteAsByte(obj.Length);
				protocolWriter.WriteAsByte(obj.Width);
				protocolWriter.WriteAsByte(obj.Height);
				break;
			case BitSize.Sixteen:
				protocolWriter.WriteAsUInt16(obj.Length);
				protocolWriter.WriteAsUInt16(obj.Width);
				protocolWriter.WriteAsUInt16(obj.Height);
				break;
			case BitSize.ThirtyTwo:
				protocolWriter.WriteAsUInt32(obj.Length);
				protocolWriter.WriteAsUInt32(obj.Width);
				protocolWriter.WriteAsUInt32(obj.Height);
				break;
			case BitSize.SixtyFour:
				protocolWriter.WriteAsUInt64(obj.Length);
				protocolWriter.WriteAsUInt64(obj.Width);
				protocolWriter.WriteAsUInt64(obj.Height);
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
		where T : struct, IBinaryInteger<T>, INumber<T>, IComparable<T>
		where TObject : IWithCoordinates<T>
	{
		switch (bitSize)
		{
			case BitSize.Eight:
				protocolWriter.WriteAsByte(obj.X);
				protocolWriter.WriteAsByte(obj.Y);
				protocolWriter.WriteAsByte(obj.Z);
				break;
			case BitSize.Sixteen:
				protocolWriter.WriteAsUInt16(obj.X);
				protocolWriter.WriteAsUInt16(obj.Y);
				protocolWriter.WriteAsUInt16(obj.Z);
				break;
			case BitSize.ThirtyTwo:
				protocolWriter.WriteAsUInt32(obj.X);
				protocolWriter.WriteAsUInt32(obj.Y);
				protocolWriter.WriteAsUInt32(obj.Z);
				break;
			case BitSize.SixtyFour:
				protocolWriter.WriteAsUInt64(obj.X);
				protocolWriter.WriteAsUInt64(obj.Y);
				protocolWriter.WriteAsUInt64(obj.Z);
				break;
			default:
				throw new ArgumentOutOfRangeException($"BitSize {bitSize} is not supported");
		}
	}
}
