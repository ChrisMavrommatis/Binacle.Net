// Ports C#: ICompressionCodec. Squeezes the body, and unsqueezes it (PROTOCOL.md §6).
//
// The wire is not pluggable — the spec fixes one codec per Version and puts no codec field on the wire — but this
// interface is. ProtocolEncoder takes a codec so every combination of widths, layout and compression is forceable
// with the body left readable (hand it noOpCodec) or really compressed (hand it deflateCodec).
//
// Both sides are async because the browser's only built-in compressor is the streaming CompressionStream API;
// Node exposes the same global. That is why ProtocolEncoder's encode/decode are async too.
export interface CompressionCodec {
	compress(body: Uint8Array): Promise<Uint8Array<ArrayBuffer>>;

	// Throws ViPaqFormatError when the bytes are not a valid stream for this codec.
	decompress(compressed: Uint8Array): Promise<Uint8Array<ArrayBuffer>>;
}
