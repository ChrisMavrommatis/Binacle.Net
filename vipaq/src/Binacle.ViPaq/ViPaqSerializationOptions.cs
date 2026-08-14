namespace Binacle.ViPaq;

// The caller-facing knobs for Serialize. Both are the encoder's choice (PROTOCOL.md §4) and both are recorded
// in the header, so a decoder is never told any of this.
public sealed class ViPaqSerializationOptions
{
	// Compresses the body (raw DEFLATE) and sets the Compressed bit. Nothing checks whether compression paid:
	// on a small pack a compressed blob can be larger, and that is still conformant (§6). Default off.
	public bool Compress { get; set; } = false;

	// Row-major or columnar item order (PROTOCOL.md §3). Columnar is usually smaller once compression runs.
	// Default row-major.
	public Layout Layout { get; set; } = Layout.RowMajor;
}
