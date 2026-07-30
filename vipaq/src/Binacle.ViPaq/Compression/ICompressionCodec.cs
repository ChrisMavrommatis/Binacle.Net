namespace Binacle.ViPaq.Compression;

// Squeezes the body, and unsqueezes it (PROTOCOL.md §6).
//
// **The wire is not pluggable. This interface is.** The spec fixes one codec per `Version` and puts no codec
// field on the wire, so a shipped blob is never ambiguous about how to inflate it — `ViPaqSerializer` names one
// codec and only that one ever reaches a caller.
//
// The interface exists for the two layers above it. `ProtocolEncoder` takes a codec so it can be handed a
// `NoOpCodec` and every combination of widths, layout and compression becomes forceable with the body still
// readable (§6.1 forbids comparing real compressed bytes). And the permanent harness measures deflate against
// gzip on every run, mirroring both onto protobuf, so both implementations stay for as long as those tables do.
//
// Do not delete an implementation because the wire settled on the other one. Do not read this interface as
// permission to put a codec field on the wire.
public interface ICompressionCodec
{
	byte[] Compress(ReadOnlySpan<byte> body);

	// Throws ViPaqFormatException when the bytes are not a valid stream for this codec.
	byte[] Decompress(ReadOnlySpan<byte> compressed);
}
