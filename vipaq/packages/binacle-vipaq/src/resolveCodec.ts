import {Header} from "./models";
import {CompressionCodec, deflateCodec, noOpCodec} from "./compression";

// Ports C#: ViPaqSerializer.ResolveCodec. The wire pins one codec by Version (PROTOCOL.md §6): raw DEFLATE when
// the compressed bit is set, otherwise a NoOp. One place decides, so encode and decode cannot disagree.
export function resolveCodec(header: Header): CompressionCodec {
	return header.compressed ? deflateCodec : noOpCodec;
}
