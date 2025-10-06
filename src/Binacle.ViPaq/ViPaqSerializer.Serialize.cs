using System.IO.Compression;
using System.Numerics;
using System.Runtime.CompilerServices;
using Binacle.ViPaq.Abstractions;

namespace Binacle.ViPaq;

public static partial class ViPaqSerializer
{
	public static byte[] Serialize<TBin, TItem, T>(
		TBin bin,
		IList<TItem> items
	)
		where T : struct, IBinaryInteger<T>, INumber<T>, IComparable<T>
		where TBin : IWithDimensions<T>
		where TItem : IWithDimensions<T>, IWithCoordinates<T>
	{
		var encodingInfo = EncodingInfoHelper.CreateEncodingInfo<TBin, TItem, T>(bin, items);
		var numberOfItems = (ushort)items.Count;

		using var memoryStream = new MemoryStream();
		using var protocolWriter = new ProtocolWriter<T>(memoryStream);

		protocolWriter.WriteUInt16(numberOfItems);
		protocolWriter.WriteDimensions<TBin, T>(bin, encodingInfo.BinDimensionsBitSize);

		foreach (var item in items)
		{
			protocolWriter.WriteDimensions<TItem, T>(item, encodingInfo.ItemDimensionsBitSize);
			protocolWriter.WriteCoordinates<TItem, T>(item, encodingInfo.ItemCoordinatesBitSize);
		}

		var shouldCompress = memoryStream.Length > byte.MaxValue;
		if (!shouldCompress)
		{
			return JoinEncodingInfoAndStream(encodingInfo, memoryStream);
		}

		encodingInfo.Version = Version.CompressedGzip;

		using var compressedStream = new MemoryStream();
		using (var compressionStream = new GZipStream(compressedStream, CompressionLevel.Optimal, leaveOpen: true))
		{
			memoryStream.Position = 0; // Reset memory stream to the beginning
			memoryStream.CopyTo(compressionStream);
		}

		return JoinEncodingInfoAndStream(encodingInfo, compressedStream);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte[] JoinEncodingInfoAndStream(EncodingInfo encodingInfo, MemoryStream memoryStream)
	{
		byte[] serializedData = memoryStream.ToArray();
		var finalData = new byte[1 + serializedData.Length];
		finalData[0] = EncodingInfoHelper.ToByte(encodingInfo);
		Buffer.BlockCopy(serializedData, 0, finalData, 1, serializedData.Length);
		return finalData;
	}
}
