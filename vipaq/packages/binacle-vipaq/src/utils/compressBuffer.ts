export async function compressBuffer(data: Uint8Array<ArrayBuffer>): Promise<Uint8Array<ArrayBuffer>> {

	const compressionStream = new CompressionStream('gzip');
	const blob = new Blob([data.buffer]);
	const compressedStream = blob.stream().pipeThrough(compressionStream);
	const reader = compressedStream.getReader();
	const chunks: Uint8Array[] = [];
	let done = false;
	while (!done) {
		const {value, done: readerDone} = await reader.read();
		if (value) chunks.push(value);
		done = readerDone;
	}
	const compressed = new Uint8Array(chunks.reduce((acc, chunk) => acc + chunk.length, 0));
	let offset = 0;
	for (const chunk of chunks) {
		compressed.set(chunk, offset);
		offset += chunk.length;
	}
	return compressed;
}
