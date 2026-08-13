import {Layout} from "./models";

// Ports C#: ViPaqSerializationOptions. The caller-facing knobs for serialize. Both are the encoder's choice
// (PROTOCOL.md §4) and both are recorded in the header, so a decoder is never told any of this.
export interface ViPaqSerializationOptions {
	// Compresses the body (raw DEFLATE) and sets the compressed bit. Nothing checks whether compression paid:
	// on a small pack a compressed blob can be larger, and that is still conformant (§6). Default off.
	compress?: boolean;

	// Row-major or columnar item order (PROTOCOL.md §3). Columnar is usually smaller once compression runs.
	// Default row-major.
	layout?: Layout;
}
