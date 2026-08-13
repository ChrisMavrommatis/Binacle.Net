namespace Binacle.ViPaq.Compression;

// Squeezes the body, and unsqueezes it (PROTOCOL.md §6).
//
// The wire is not pluggable; this interface is. The spec fixes one codec per `Version` and puts no codec field
// on the wire. The interface exists for tests (a `NoOpCodec` makes every width/layout/compression combination
// forceable with the body still readable) and for the harness, which measures deflate against gzip on every run.
//
// Do not delete an implementation because the wire settled on the other one. Do not read this interface as
// permission to put a codec field on the wire.
public interface ICompressionCodec
{
	byte[] Compress(ReadOnlySpan<byte> body);

	// Throws ViPaqFormatException when the bytes are not a valid stream for this codec.
	byte[] Decompress(ReadOnlySpan<byte> compressed);
}
