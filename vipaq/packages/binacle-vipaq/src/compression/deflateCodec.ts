import {CompressionCodec} from "./compressionCodec";
import {runStream} from "./runStream";
import {ViPaqFormatError} from "../utils";

// Ports C#: DeflateCodec. Raw DEFLATE (RFC 1951), the same stream C#'s DeflateStream writes. The '-raw' variant
// is required: plain 'deflate' adds a zlib header (RFC 1950) that C# does not produce, so it would not interop.
// Node exposes CompressionStream as a global, so this one path serves the browser and Node.
//
// Compressed bytes are NOT compared across engines - different zlib builds differ. The cross-language guarantee
// is decode-to-input (PROTOCOL.md §6.1), proven by the round-trip tests, not taken on trust.
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
