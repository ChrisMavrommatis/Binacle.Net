using System.Numerics;
using Binacle.Geometry;
using Binacle.ViPaq.Compression;
using Binacle.ViPaq.Helpers;

namespace Binacle.ViPaq;

// The choosing layer, and the only entry point a caller needs.
//
// `ProtocolEncoder` is blind: it is handed a header and obeys it. Something has to decide that header, and this
// is where that goes. Three things are the encoder's choice, and all three are recorded in the header so a
// decoder never guesses (PROTOCOL.md §4 "Selection", §6, decisions.md D14):
//
//   - **Widths** — the narrowest that holds each section, sized independently. A big bin can hold small items
//     at large coordinates, so the three sections genuinely disagree (findings.md: Bischoff packs to 16/8/16).
//     With no items, both item widths must be `Eight` (§4).
//   - **Layout** — unmeasured, so `RowMajor` always. See `Header.Create`.
//   - **Compressed** — always false, for now. See below.
//
// Deserialize is the easy half: split off the two header bytes — which are never compressed — and hand the
// encoder the header plus everything after it. The uint16 item count lives *inside* the compressed body (§3),
// so only the encoder can read it (§7, steps 4-5).
// **This writes uncompressed blobs only, and cannot read a compressed one.**
//
// Two things are unsettled. `PROTOCOL.md` §6 names no codec, so there is nothing to pin one to. And D7 says to
// encode both ways and keep the shorter blob, which costs a whole second compression on every call — a cost
// nobody has measured. Benchmark both before wiring either up.
//
// So the encoder gets a NoOpCodec, and `Header.Create` never sets `Compressed`. That is honest: the codec is not
// chosen by default, it is not chosen at all.
public static class ViPaqSerializer
{
	public static byte[] Serialize<TBin, TItem, T>(TBin bin, IReadOnlyList<TItem> items)
		where T : struct, IBinaryInteger<T>
		where TBin : IWithReadOnlyDimensions<T>
		where TItem : IWithReadOnlyDimensions<T>, IWithReadOnlyCoordinates<T>
	{
		ArgumentNullException.ThrowIfNull(bin);
		ArgumentNullException.ThrowIfNull(items);
		ValidationHelper.ThrowIfTooManyItems(items);

		var header = Header.Create<TBin, TItem, T>(bin, items);
		var encoder = new ProtocolEncoder(new NoOpCodec());

		return encoder.Encode<TBin, TItem, T>(header, bin, items);
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

		// A conformant blob, and we still cannot read it: the codec is unchosen, so there is nothing to inflate
		// it with. Say that plainly rather than let a NoOpCodec hand the decoder deflated bytes.
		if (header.Compressed)
		{
			throw new NotSupportedException(
				"This blob is compressed. The ViPaq compression codec is not chosen yet (PROTOCOL.md §6)"
			);
		}

		var encoder = new ProtocolEncoder(new NoOpCodec());

		return encoder.Decode<TBin, TItem, T>(header, data[Header.ByteCount..]);
	}
}
