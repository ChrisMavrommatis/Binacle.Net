// mirrors src/utils/compressBuffer.ts
// TS-specific: gzip runs through the async Web Streams CompressionStream, not C#'s synchronous
// GZipStream. We do NOT assert exact gzip bytes (engines differ — that is Session 2); we assert the
// output is real gzip that decompresses back to the input.
import {compressBuffer} from "../../src/utils";

// Independent gunzip so this test does not lean on our own decoder.
async function gunzip(data: Uint8Array<ArrayBuffer>): Promise<Uint8Array> {
	const stream = new Blob([data.buffer]).stream().pipeThrough(new DecompressionStream("gzip"));
	const reader = (stream as ReadableStream<Uint8Array>).getReader();
	const chunks: Uint8Array[] = [];
	for (;;) {
		const {value, done} = await reader.read();
		if (value) chunks.push(value);
		if (done) break;
	}
	const out = new Uint8Array(chunks.reduce((n, c) => n + c.length, 0));
	let offset = 0;
	for (const c of chunks) {
		out.set(c, offset);
		offset += c.length;
	}
	return out;
}

const body = new Uint8Array(Array.from({length: 300}, (_, i) => i % 256));

describe("compressBuffer", () => {
	test("produces gzip that decompresses back to the input", async () => {
		const compressed = await compressBuffer(body);
		const restored = await gunzip(compressed);
		expect(Array.from(restored)).toEqual(Array.from(body));
	});

	test("starts with the gzip magic bytes", async () => {
		const compressed = await compressBuffer(body);
		expect([compressed[0], compressed[1]]).toEqual([0x1f, 0x8b]);
	});
});
