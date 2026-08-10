using System.Numerics;
using Binacle.Geometry;
using Binacle.ViPaq.Helpers;

namespace Binacle.ViPaq;

// The two header bytes (PROTOCOL.md §2). This is both what the encoder is told to do and what lands on the
// wire — there is no separate directive type, because the spec already makes the header describe the blob
// completely.
//
//   Byte 0 — form                                Byte 1 — widths
//   [Version][Compressed][Layout][reserved]      [Bin dims][Item dims][Item coords][reserved]
//   [2 bits ][1 bit     ][1 bit ][4 bits  ]      [2 bits  ][2 bits   ][2 bits     ][2 bits  ]
//
// Every field is `init`, `Compressed` included: the blind encoder obeys that bit rather than deciding it.
// Anything wanting a different form builds a second header instead of mutating this one — which is how the
// measurement harness races codecs (PROTOCOL.md §6, decisions.md D7). The serializer itself does not race;
// its `Compress` option is a straight on/off.
internal readonly record struct Header
{
	// The header is always two bytes, and it is never compressed.
	public const int ByteCount = 2;

	// Not `required`: `Version1` is 0, so the default is the only version this implementation writes. Codes
	// 1-3 are reserved (PROTOCOL.md §2.3) and a decoder rejects them.
	public Version Version { get; init; }

	public required bool Compressed { get; init; }

	public required Layout Layout { get; init; }

	public required Width BinDimensionsWidth { get; init; }

	public required Width ItemDimensionsWidth { get; init; }

	public required Width ItemCoordinatesWidth { get; init; }

	// The header for this data, at the library's default form: the narrowest widths that hold each section
	// (PROTOCOL.md §4), uncompressed and row-major. The mirror of `FromBytes` — that reads a header off the
	// wire, this derives one from the values.
	//
	//   - Widths are derived here and nowhere else (decisions.md D14). `Serialize` calls this to write a blob
	//     and the measurement harness calls it to force a mode; neither re-derives a width.
	//   - The three sections are sized independently, because they genuinely disagree: a big bin can hold small
	//     items at large coordinates (findings.md: Bischoff packs to 16/8/16).
	//   - With no items, both item widths stay `Eight` — what §4 requires of an empty blob.
	//   - `Compressed` and `Layout` stay at their defaults (see the note on the type). Layout is unmeasured, so
	//     row-major. A caller wanting another form `with`-s those two bits.
	public static Header Create<TBin, TItem, T>(TBin bin, IReadOnlyList<TItem> items)
		where T : struct, IBinaryInteger<T>
		where TBin : IWithReadOnlyDimensions<T>
		where TItem : IWithReadOnlyDimensions<T>, IWithReadOnlyCoordinates<T>
	{
		var itemDimensionsWidth = Width.Eight;
		var itemCoordinatesWidth = Width.Eight;

		foreach (var item in items)
		{
			itemDimensionsWidth = WidthHelper.Widest(
				itemDimensionsWidth,
				WidthHelper.GetDimensionsWidth<TItem, T>(item)
				);

			itemCoordinatesWidth = WidthHelper.Widest(
				itemCoordinatesWidth,
				WidthHelper.GetCoordinatesWidth<TItem, T>(item)
				);
		}

		return new Header
		{
			Compressed = false,
			Layout = Layout.RowMajor,
			BinDimensionsWidth = WidthHelper.GetDimensionsWidth<TBin, T>(bin),
			ItemDimensionsWidth = itemDimensionsWidth,
			ItemCoordinatesWidth = itemCoordinatesWidth
		};
	}

	// Writes the two bytes (PROTOCOL.md §2.1, §2.2). `destination` must be at least ByteCount long.
	public void ToBytes(Span<byte> destination)
	{
		this.ThrowIfNotWritable();

		var compressedBit = this.Compressed ? 1 : 0;

		destination[0] = (byte)(
			((byte)this.Version << 6)
			| (compressedBit << 5)
			| ((byte)this.Layout << 4)
			);

		destination[1] = (byte)(
			((byte)this.BinDimensionsWidth << 6)
			| ((byte)this.ItemDimensionsWidth << 4)
			| ((byte)this.ItemCoordinatesWidth << 2)
			);
	}

	// PROTOCOL.md §7, steps 2 and 3. Every rejection here means a malformed blob, not a caller bug.
	public static Header FromBytes(byte formByte, byte widthsByte)
	{
		var version = (Version)((formByte & 0b1100_0000) >> 6);
		if (version != Version.Version1)
		{
			throw new ViPaqFormatException(
				$"Unsupported version {(int)version}, this implementation reads {(int)Version.Version1}"
				);
		}

		if ((formByte & 0b0000_1111) != 0)
		{
			throw new ViPaqFormatException("Reserved bits 3-0 of header byte 0 must be zero");
		}

		if ((widthsByte & 0b0000_0011) != 0)
		{
			throw new ViPaqFormatException("Reserved bits 1-0 of header byte 1 must be zero");
		}

		return new Header
		{
			Version = version,
			Compressed = (formByte & 0b0010_0000) != 0,
			Layout = (Layout)((formByte & 0b0001_0000) >> 4),
			BinDimensionsWidth = ToWidth((widthsByte & 0b1100_0000) >> 6, "bin dimensions"),
			ItemDimensionsWidth = ToWidth((widthsByte & 0b0011_0000) >> 4, "item dimensions"),
			ItemCoordinatesWidth = ToWidth((widthsByte & 0b0000_1100) >> 2, "item coordinates")
		};
	}

	// A body's length is fixed once the header and the item count are known — no field is variable-length.
	// That one number is also the whole of decode's structural check: a body of exactly this length cannot be
	// truncated and cannot have bytes left over (PROTOCOL.md §7, steps 1 and 8).
	public int GetBodyLength(int itemCount)
	{
		return sizeof(ushort) + this.GetContentsLength(itemCount);
	}

	// The body minus its leading uint16 item count: the bin dimensions, then the items. This is what the
	// encoder writes and the decoder reads once the count is already known.
	public int GetContentsLength(int itemCount)
	{
		var bytesPerItem = 3 * (
			WidthHelper.ByteCount(this.ItemDimensionsWidth)
			+ WidthHelper.ByteCount(this.ItemCoordinatesWidth)
			);

		return (3 * WidthHelper.ByteCount(this.BinDimensionsWidth))
			+ (itemCount * bytesPerItem);
	}

	// A header a caller built by hand can still be unwritable. Both checks are encode-side caller errors, not
	// malformed input.
	private void ThrowIfNotWritable()
	{
		if (this.Version != Version.Version1)
		{
			throw new ArgumentOutOfRangeException(
				nameof(this.Version),
				this.Version,
				$"This implementation writes only {Version.Version1}"
				);
		}

		// An encoder must never write a reserved width code (PROTOCOL.md §4). ByteCount throws on one, so a
		// reserved width cannot reach the wire.
		WidthHelper.ByteCount(this.BinDimensionsWidth);
		WidthHelper.ByteCount(this.ItemDimensionsWidth);
		WidthHelper.ByteCount(this.ItemCoordinatesWidth);
	}

	// Width codes 2 and 3 are reserved. A decoder must reject them (PROTOCOL.md §4).
	private static Width ToWidth(int code, string section)
	{
		var width = (Width)code;
		return width switch
		{
			Width.Eight or Width.Sixteen => width,
			_ => throw new ViPaqFormatException($"Reserved width code {code} for {section}")
		};
	}
}
