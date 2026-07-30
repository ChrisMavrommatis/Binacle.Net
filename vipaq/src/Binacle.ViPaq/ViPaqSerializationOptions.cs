namespace Binacle.ViPaq;

// The caller-facing knobs for Serialize. Both default to the smallest, safest choice: no compression, row-major.
// The header records whatever was actually used, so a decoder never needs to be told any of this.
//
// Compression and layout are the encoder's choice (PROTOCOL.md §4, decisions.md D16), exposed here rather than
// pinned, so the default stays raw and row-major and a caller opts in when they want a smaller token.
public sealed class ViPaqSerializationOptions
{
	// When true, Serialize compresses the body (raw DEFLATE) and sets the Compressed bit; when false it writes the
	// body raw. It does not check whether compression paid — on a small pack a compressed blob can be larger, and
	// that is still conformant (§6). Default off.
	public bool Compress { get; set; } = false;

	// Row-major or columnar item order (PROTOCOL.md §3). Columnar is usually smaller once compression runs, but it
	// is never chosen for you — set it here. Default row-major.
	public Layout Layout { get; set; } = Layout.RowMajor;
}
