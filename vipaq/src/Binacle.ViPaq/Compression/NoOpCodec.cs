namespace Binacle.ViPaq.Compression;

// Hands the bytes straight back. Not a wire codec: a blob with `Compressed = 1` must carry a real compressed
// stream (PROTOCOL.md §6).
//
// It makes `ProtocolEncoder` testable on its own. Set `Compressed = 1` and hand it this codec, and the
// compressed path runs end to end with the body still readable byte for byte - impossible through a real codec,
// because compressed bytes must never be compared (§6.1).
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
