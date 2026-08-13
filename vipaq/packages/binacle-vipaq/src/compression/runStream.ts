// Runs bytes through a Web-Streams transform (CompressionStream / DecompressionStream) and collects the whole
// output into one array. Shared by deflateCodec and gzipCodec.
//
// The write is left floating on purpose: CompressionStream applies backpressure, so write() may not resolve
// until the reader drains it, and awaiting it before reading would deadlock. Invalid input rejects BOTH sides;
// the read loop surfaces the real error, and the no-op catch keeps the write-side rejection from going
// unhandled.
export async function runStream(
	data: Uint8Array,
	stream: CompressionStream | DecompressionStream,
): Promise<Uint8Array<ArrayBuffer>> {
	const writer = stream.writable.getWriter();
	// Copy into a fresh ArrayBuffer-backed view: the stream writer's type rejects a possibly-shared buffer.
	const pumped = writer.write(new Uint8Array(data)).then(() => writer.close());
	void pumped.catch(() => undefined);

	const reader = stream.readable.getReader();
	const chunks: Uint8Array[] = [];
	let total = 0;
	for (;;) {
		const {done, value} = await reader.read();
		if (done) {
			break;
		}
		const chunk = value as Uint8Array;
		chunks.push(chunk);
		total += chunk.length;
	}
	await pumped;

	const output = new Uint8Array(total);
	let offset = 0;
	for (const chunk of chunks) {
		output.set(chunk, offset);
		offset += chunk.length;
	}
	return output;
}
