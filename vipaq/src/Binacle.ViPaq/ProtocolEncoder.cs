using System.Buffers.Binary;
using System.Numerics;
using Binacle.Geometry;
using Binacle.ViPaq.Compression;
using Binacle.ViPaq.Helpers;
using Binacle.ViPaq.Layouts;

namespace Binacle.ViPaq;

// The blind layer (PROTOCOL.md §1, §3, §6, §7). Handed a header, it obeys it and decides nothing; the
// serializer chooses the header. It never re-derives a width from the values it reads (§4), which is what lets
// a deliberately-too-wide blob round-trip.
//
// The codec is a constructor argument, so tests can force every combination of widths, layout and compression
// with a dummy codec. `ViPaqSerializer.ResolveCodec` picks the real one.
internal sealed class ProtocolEncoder
{
	private readonly ICompressionCodec compressionCodec;

	public ProtocolEncoder(ICompressionCodec compressionCodec)
	{
		ArgumentNullException.ThrowIfNull(compressionCodec);
		this.compressionCodec = compressionCodec;
	}

	// Produces a whole blob, obeying the header's Compressed bit. A header whose widths cannot hold the data is
	// an argument error, not a silent truncation (§8, encode side).
	public byte[] Encode<TBin, TItem, T>(Header header, TBin bin, IReadOnlyList<TItem> items)
		where T : struct, IBinaryInteger<T>
		where TBin : IWithReadOnlyDimensions<T>
		where TItem : IWithReadOnlyDimensions<T>, IWithReadOnlyCoordinates<T>
	{
		ArgumentNullException.ThrowIfNull(bin);
		ArgumentNullException.ThrowIfNull(items);

		ValidationHelper.ThrowIfTooManyItems(items);
		ValidationHelper.ThrowIfEmptyAndNotEightBits(items, header);
		ValidationHelper.ThrowIfHeaderCannotHold<TBin, TItem, T>(bin, items, header);

		var memoryStream = new MemoryStream(header.GetContentsLength(items.Count));
		var layoutEncoder = LayoutCodecFactory<T>.CreateEncoder(header.Layout);

		// The writer owns the stream and disposes it. MemoryStream.ToArray still works afterwards.
		using (var protocolWriter = new ProtocolWriter<T>(memoryStream))
		{
			protocolWriter.WriteValue(bin.Length, header.BinDimensionsWidth);
			protocolWriter.WriteValue(bin.Width, header.BinDimensionsWidth);
			protocolWriter.WriteValue(bin.Height, header.BinDimensionsWidth);
			layoutEncoder.Write(protocolWriter, items, header);
		}

		var contents = memoryStream.ToArray();
		var body = PrependItemCount(contents, (ushort)items.Count);

		var compressedBody = this.compressionCodec.Compress(body);
		return PrependHeader(compressedBody, header);
	}

	// `rest` is everything after the two header bytes, still compressed if the header says so. The uint16 item
	// count is inside it (§3), so it cannot be read until the body is inflated (§7, step 4).
	public (TBin, IList<TItem>) Decode<TBin, TItem, T>(Header header, byte[] rest)
		where T : struct, IBinaryInteger<T>
		where TBin : IWithDimensions<T>, new()
		where TItem : IWithDimensions<T>, IWithCoordinates<T>, new()
	{
		ArgumentNullException.ThrowIfNull(rest);

		// The generic argument has to be able to hold what the header declares.
		HeaderHelper.ThrowOnInvalidHeader<T>(header);

		// The codec was resolved from the header: NoOp passes a raw body through (§7, step 4).
		var body = this.compressionCodec.Decompress(rest);

		ValidationHelper.ThrowIfBodyHasNoItemCount(body);

		var numberOfItems = BinaryPrimitives.ReadUInt16LittleEndian(body);
		var contents = body[sizeof(ushort)..];

		// One length check covers truncation and trailing bytes (§7, steps 1 and 8), so no read below needs a
		// bounds guard.
		ValidationHelper.ThrowIfContentsIsNotExactlyAsLongAsDeclared(contents, header, numberOfItems);

		// A byte[] keeps ProtocolReader on its MemoryStream fast path; a live decompression stream would hand it
		// one or two bytes at a time.
		using var memoryStream = new MemoryStream(contents);
		using var protocolReader = new ProtocolReader<T>(memoryStream);
		var bin = new TBin
		{
			Length = protocolReader.ReadValue(header.BinDimensionsWidth),
			Width = protocolReader.ReadValue(header.BinDimensionsWidth),
			Height = protocolReader.ReadValue(header.BinDimensionsWidth)
		};
		
		var items = new TItem[numberOfItems];
		for (var index = 0; index < numberOfItems; index++)
		{
			items[index] = new TItem();
		}

		var layoutDecoder = LayoutCodecFactory<T>.CreateDecoder(header.Layout);
		layoutDecoder.Read(protocolReader, items, header);

		return (bin, items);
	}

	// The item count is always a uint16, whatever the widths say (PROTOCOL.md §3).
	private static byte[] PrependItemCount(byte[] contents, ushort numberOfItems)
	{
		var body = new byte[sizeof(ushort) + contents.Length];
		BinaryPrimitives.WriteUInt16LittleEndian(body, numberOfItems);
		contents.CopyTo(body, sizeof(ushort));
		return body;
	}

	// The header is never compressed, so it goes on the front last (PROTOCOL.md §1).
	private static byte[] PrependHeader(byte[] body, Header header)
	{
		var blob = new byte[Header.ByteCount + body.Length];
		header.ToBytes(blob);
		body.CopyTo(blob, Header.ByteCount);
		return blob;
	}
}
