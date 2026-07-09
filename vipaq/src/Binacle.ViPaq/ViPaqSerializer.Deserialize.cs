using System.IO.Compression;
using System.Numerics;
using Binacle.Geometry;
using Binacle.ViPaq.Helpers;
using Version = Binacle.ViPaq.Version;

namespace Binacle.ViPaq;

public static partial class ViPaqSerializer
{
	public static (TBin, IList<TItem>) Deserialize<TBin, TItem, T>(
		byte[] data
	)
		where T : struct, IBinaryInteger<T>
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
		var encodingInfo = EncodingInfoHelper.FromByte(firstByte);
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

	private static Stream GetDecodingDataStream(MemoryStream stream, EncodingInfo encodingInfo)
	{
		if (encodingInfo.Version == Version.Uncompressed)
		{
			return stream;
		}

		if (encodingInfo.Version == Version.CompressedGzip)
		{
			// Decompress the whole body once into a MemoryStream, so ProtocolReader hits its MemoryStream fast
			// path instead of reading each value one or two bytes at a time off a live GZipStream (~10× slower
			// on 8/16-bit data). See .agents/plans/vipaq/02-decode-fix.md.
			using var gzipStream = new GZipStream(stream, CompressionMode.Decompress);
			var decompressed = new MemoryStream();
			gzipStream.CopyTo(decompressed);
			decompressed.Position = 0;
			return decompressed;
		}

		throw new NotSupportedException($"Version {encodingInfo.Version} is not supported");
	}
}
