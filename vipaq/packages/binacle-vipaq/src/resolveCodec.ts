import {Header} from "./models";
import {CompressionCodec, deflateCodec, noOpCodec} from "./compression";

// Ports C#: ViPaqSerializer.ResolveCodec. The codec a header's body is written and read with. The wire pins one
// codec by Version (PROTOCOL.md §6): raw DEFLATE when the compressed bit is set, otherwise a NoOp that passes
// the raw body through. One place decides which — the same rule for encode and decode.
export function resolveCodec(header: Header): CompressionCodec {
	return header.compressed ? deflateCodec : noOpCodec;
}
