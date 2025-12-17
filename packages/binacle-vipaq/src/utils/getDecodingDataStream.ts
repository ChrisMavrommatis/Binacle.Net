import {EncodingInfo, Version} from "../models";

export async function getDecodingDataStream(data: Uint8Array<ArrayBuffer>, encodingInfo: EncodingInfo): Promise<DataView<ArrayBuffer>> {

	if (encodingInfo.version == Version.Uncompressed) {
		return new DataView(data.buffer);
	}

	if (encodingInfo.version == Version.CompressedGzip) {
		const blob = new Blob([data.buffer]);
		const ds = new DecompressionStream('gzip');
		const decompressedStream = blob.stream().pipeThrough(ds);
		const reader = decompressedStream.getReader();
		const chunks: Uint8Array[] = [];
		let done = false;
		while (!done) {
			const {value, done: readerDone} = await reader.read();
			if (value) chunks.push(value);
			done = readerDone;
		}
		const decompressed = new Uint8Array(chunks.reduce((acc, chunk) => acc + chunk.length, 0));
		let offset = 0;
		for (const chunk of chunks) {
			decompressed.set(chunk, offset);
			offset += chunk.length;
		}
		return new DataView(decompressed.buffer);
	}

	throw new Error(`Version ${encodingInfo.version} is not supported`);
}
