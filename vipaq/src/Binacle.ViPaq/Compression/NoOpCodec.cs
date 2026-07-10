namespace Binacle.ViPaq.Compression;

// Hands the bytes straight back. Not a wire codec — no conforming blob is ever written with it, because a blob
// with `Compressed = 1` must carry a real compressed stream (PROTOCOL.md §6).
//
// It exists to make `ProtocolEncoder` testable on its own. Set `Compressed = 1` in the header, hand it this
// codec, and the compressed path runs end to end with the body left readable: the framing, the item count and
// the contents can all be checked byte for byte, which is impossible through a real codec because compressed
// bytes must never be compared (§6.1).
//
// Compress and Decompress are exact inverses, so a round-trip through this codec proves the framing without
// proving anything about compression.
internal sealed class NoOpCodec : ICompressionCodec
{
	public byte[] Compress(ReadOnlySpan<byte> body)
	{
		return body.ToArray();
	}

	public byte[] Decompress(ReadOnlySpan<byte> compressed)
	{
		return compressed.ToArray();
	}
}
