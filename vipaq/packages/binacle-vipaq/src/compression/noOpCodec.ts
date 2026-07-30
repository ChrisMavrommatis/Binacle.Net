import {CompressionCodec} from "./compressionCodec";

// Ports C#: NoOpCodec. Hands the bytes straight back (a copy, so callers can't alias the source). Not a wire
// codec — no conforming blob is written with it, because Compressed = 1 must carry a real compressed stream
// (PROTOCOL.md §6). It exists to make ProtocolEncoder testable: set the compressed bit, hand it this codec, and
// the compressed path runs end to end with the framing and contents still readable byte for byte.
export const noOpCodec: CompressionCodec = {
	async compress(body: Uint8Array): Promise<Uint8Array<ArrayBuffer>> {
		return new Uint8Array(body);
	},

	async decompress(compressed: Uint8Array): Promise<Uint8Array<ArrayBuffer>> {
		return new Uint8Array(compressed);
	},
};
