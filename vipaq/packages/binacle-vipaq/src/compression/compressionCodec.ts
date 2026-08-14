// Ports C#: ICompressionCodec. Squeezes the body, and unsqueezes it (PROTOCOL.md §6).
//
// The wire is not pluggable (one codec per Version, no codec field on the wire); this interface is.
// ProtocolEncoder takes a codec so tests can force every width/layout/compression combination with the body
// left readable (noOpCodec) or really compressed (deflateCodec).
//
// Both sides are async because the browser's only built-in compressor is the streaming CompressionStream API.
// That is why ProtocolEncoder's encode/decode are async too.
export interface CompressionCodec {
	compress(body: Uint8Array): Promise<Uint8Array<ArrayBuffer>>;

	// Throws ViPaqFormatError when the bytes are not a valid stream for this codec.
	decompress(compressed: Uint8Array): Promise<Uint8Array<ArrayBuffer>>;
}
