import {CompressionCodec} from "./compressionCodec";
import {runStream} from "./runStream";
import {ViPaqFormatError} from "../utils";

// Ports C#: GzipCodec. Gzip (RFC 1952) — the same DEFLATE stream as deflateCodec, wrapped in ~18 bytes of magic,
// mtime, OS byte and a CRC trailer. The wrapper buys nothing here (the header already says the body is
// compressed, and the body knows its own length), so deflate is the pick; gzip is kept so it can be raced against
// raw DEFLATE, mirroring C#. CompressionStream('gzip') pairs with C#'s GZipStream.
export const gzipCodec: CompressionCodec = {
	async compress(body: Uint8Array): Promise<Uint8Array<ArrayBuffer>> {
		return runStream(body, new CompressionStream("gzip"));
	},

	async decompress(compressed: Uint8Array): Promise<Uint8Array<ArrayBuffer>> {
		try {
			return await runStream(compressed, new DecompressionStream("gzip"));
		} catch (error) {
			throw new ViPaqFormatError("The compressed body is not a valid gzip stream");
		}
	},
};
