using System.Numerics;
using Binacle.Geometry;
using Binacle.ViPaq.Compression;
using Binacle.ViPaq.Helpers;

namespace Binacle.ViPaq;

// The choosing layer, and the only entry point a caller needs.
//
// `ProtocolEncoder` is blind: it is handed a header and obeys it. Something has to decide that header, and this
// is where that goes. Three things are the encoder's choice, all recorded in the header so a decoder never
// guesses (PROTOCOL.md §4 "Selection", §6, decisions.md D14):
//
//   - **Widths** — the narrowest that holds each section, sized independently. A big bin can hold small items
//     at large coordinates, so the three sections genuinely disagree (findings.md: Bischoff packs to 16/8/16).
//     With no items, both item widths must be `Eight` (§4).
//   - **Layout** — the caller's choice through `ViPaqSerializationOptions`, default `RowMajor` (D16).
//   - **Compressed** — the caller's choice too, default off. The codec follows from the header (`ResolveCodec`):
//     raw DEFLATE when set, a pass-through NoOp when not. Pinned by `Version` (§6, D16).
//
// Both directions are the same three lines: build/read the header, resolve the codec from it, hand both to the
// encoder. The uint16 item count lives *inside* the body (§3), so only the encoder can read it (§7, steps 4-5).
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

		// Create picks the widths; the caller's options set the layout and whether to compress. The width rule
		// has exactly one home (D14) — never re-derived.
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

		// FromBytes rejects an unsupported version, a set reserved bit, and a reserved width code (§7, steps 2
		// and 3), so nothing more is checked here. Everything after the two header bytes is the encoder's.
		var header = Header.FromBytes(data[0], data[1]);
		var codec = ResolveCodec(header);
		return new ProtocolEncoder(codec).Decode<TBin, TItem, T>(header, data[Header.ByteCount..]);
	}

	// The codec a header's body is written and read with. The wire pins one codec by Version (PROTOCOL.md §6,
	// D16): raw DEFLATE when the Compressed bit is set, otherwise a NoOp that passes the raw body through. One
	// place decides which — the same rule for encode and decode.
	private static ICompressionCodec ResolveCodec(Header header)
		=> header.Compressed ? new DeflateCodec() : new NoOpCodec();
}
