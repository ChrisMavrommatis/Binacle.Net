import {Layout} from "./models";

// Ports C#: ViPaqSerializationOptions. The caller-facing knobs for serialize. Both default to the smallest, safest
// choice: no compression, row-major. The header records whatever was actually used, so a decoder never needs any
// of this.
//
// Compression and layout are the encoder's choice (PROTOCOL.md §4), exposed here rather than
// pinned, so the default stays raw and row-major and a caller opts in when they want a smaller token.
export interface ViPaqSerializationOptions {
	// When true, serialize compresses the body (raw DEFLATE) and sets the compressed bit; when false it writes the
	// body raw. It does not check whether compression paid — on a small pack a compressed blob can be larger, and
	// that is still conformant (§6). Default off.
	compress?: boolean;

	// Row-major or columnar item order (PROTOCOL.md §3). Columnar is usually smaller once compression runs, but
	// it is never chosen for you — set it here. Default row-major.
	layout?: Layout;
}
