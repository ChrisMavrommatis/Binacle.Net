using System.IO.Compression;
using System.Numerics;
using System.Runtime.CompilerServices;
using Binacle.ViPaq.Abstractions;
using Binacle.ViPaq.Helpers;
using Binacle.ViPaq.Models;
using Version = Binacle.ViPaq.Models.Version;

namespace Binacle.ViPaq;

public static partial class ViPaqSerializer
{
	internal static (TBin, IList<TItem>) DeserializeInternal<TBin, TItem, T>(
		byte[] data
	)
		where T : struct, IBinaryInteger<T>, INumber<T>, IComparable<T>
		where TBin : IWithDimensions<T>, new()
		where TItem : IWithDimensions<T>, IWithCoordinates<T>, new()
	{
		if (data == null || data.Length < 1)
		{
			throw new ArgumentException("Data is invalid or empty.", nameof(data));
		}

		using var memoryStream = new MemoryStream(data);

		// Read the first byte (encoding info) before any decompression
		var firstByte = (byte)memoryStream.ReadByte();
		var encodingInfo = EncodingInfo.FromByte(firstByte);
		EncodingInfoHelper.ThrowOnInvalidEncodingInfo<T>(encodingInfo);

		// Determine if the data is compressed
		using var dataStream = GetDecodingDataStream(memoryStream, encodingInfo);

		using var protocolReader = new ProtocolReader<T>(dataStream);

		var numberOfItems = protocolReader.ReadUInt16();

		var bin = new TBin();
		protocolReader.ReadDimensions<TBin, T>(ref bin, encodingInfo.BinDimensionsBitSize);

		var items = new List<TItem>();
		for (int i = 0; i < numberOfItems; i++)
		{
			var item = new TItem();
			protocolReader.ReadDimensions<TItem, T>(ref item, encodingInfo.ItemDimensionsBitSize);
			protocolReader.ReadCoordinates<TItem, T>(ref item, encodingInfo.ItemCoordinatesBitSize);
			items.Add(item);
		}

		return (bin, items);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Stream GetDecodingDataStream(MemoryStream stream, EncodingInfo encodingInfo)
	{
		if (encodingInfo.Version == Version.Uncompressed)
		{
			return stream;
		}

		if (encodingInfo.Version == Version.CompressedGzip)
		{
			return new GZipStream(stream, CompressionMode.Decompress);
		}

		throw new NotSupportedException($"Version {encodingInfo.Version} is not supported");
	}
}
