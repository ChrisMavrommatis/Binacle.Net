using System.Numerics;
using Binacle.Geometry;
using Binacle.ViPaq.Compression;
using Binacle.ViPaq.Helpers;

namespace Binacle.ViPaq;

// The choosing layer, and the only entry point a caller needs. `ProtocolEncoder` is blind; this decides the
// header it obeys. Three choices, all recorded in the header so a decoder never guesses (PROTOCOL.md §4, §6):
//
//   - Widths - derived by `Header.Create`, narrowest that holds each section.
//   - Layout - the caller's, through `ViPaqSerializationOptions`. Default `RowMajor`.
//   - Compressed - the caller's. Default off; the codec follows from the header (`ResolveCodec`).
public static class ViPaqSerializer
{
	public static byte[] Serialize<TBin, TItem, T>(
		TBin bin,
		IReadOnlyList<TItem> items,
		Action<ViPaqSerializationOptions>? configure = null)
		where T : struct, IBinaryInteger<T>
		where TBin : IWithReadOnlyDimensions<T>
		where TItem : IWithReadOnlyDimensions<T>, IWithReadOnlyCoordinates<T>
	{
		ArgumentNullException.ThrowIfNull(bin);
		ArgumentNullException.ThrowIfNull(items);
		ValidationHelper.ThrowIfTooManyItems(items);

		var options = new ViPaqSerializationOptions();
		configure?.Invoke(options);

		// Create picks the widths; the caller's options set the layout and whether to compress.
		var header = Header.Create<TBin, TItem, T>(bin, items) with
		{
			Layout = options.Layout,
			Compressed = options.Compress
		};
		var codec = ResolveCodec(header);
		return new ProtocolEncoder(codec).Encode<TBin, TItem, T>(header, bin, items);
	}

	public static (TBin, IList<TItem>) Deserialize<TBin, TItem, T>(byte[] data)
		where T : struct, IBinaryInteger<T>
		where TBin : IWithDimensions<T>, new()
		where TItem : IWithDimensions<T>, IWithCoordinates<T>, new()
	{
		ValidationHelper.ThrowIfNotAWholeBlob(data);

		// FromBytes rejects an unsupported version, a set reserved bit and a reserved width code (§7, steps 2
		// and 3), so nothing more is checked here.
		var header = Header.FromBytes(data[0], data[1]);
		var codec = ResolveCodec(header);
		return new ProtocolEncoder(codec).Decode<TBin, TItem, T>(header, data[Header.ByteCount..]);
	}

	// The wire pins one codec by Version (PROTOCOL.md §6): raw DEFLATE when the Compressed bit is set, otherwise
	// a NoOp. One place decides, so encode and decode cannot disagree.
	private static ICompressionCodec ResolveCodec(Header header)
		=> header.Compressed ? new DeflateCodec() : new NoOpCodec();
}
