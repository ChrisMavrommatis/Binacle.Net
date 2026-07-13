import {CompressionCodec} from "./compressionCodec";
import {runStream} from "./runStream";
import {ViPaqFormatError} from "../utils";

// Ports C#: DeflateCodec. Raw DEFLATE (RFC 1951), no wrapper — the same stream C#'s DeflateStream writes, which
// is what makes a blob portable across the two languages. The '-raw' variant is required: plain 'deflate' adds a
// zlib header (RFC 1950) that C# does not produce, so it would not interop. Node exposes CompressionStream as a
// global, so this one code path serves both the browser and Node.
//
// Compressed bytes are NOT compared across engines (different zlib builds differ); the cross-language guarantee
// is decode-to-input (PROTOCOL.md §6.1). Raw DEFLATE is a standard stream, so C#'s output inflates here and ours
// inflates there — proven only by the round-trip tests, not taken on trust.
export const deflateCodec: CompressionCodec = {
	async compress(body: Uint8Array): Promise<Uint8Array<ArrayBuffer>> {
		return runStream(body, new CompressionStream("deflate-raw"));
	},

	async decompress(compressed: Uint8Array): Promise<Uint8Array<ArrayBuffer>> {
		try {
			return await runStream(compressed, new DecompressionStream("deflate-raw"));
		} catch (error) {
			throw new ViPaqFormatError("The compressed body is not a valid DEFLATE stream");
		}
	},
};
