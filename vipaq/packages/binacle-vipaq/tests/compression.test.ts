// Unit coverage for the compression codecs and the ProtocolEncoder compressed path. The cross-language proof —
// each language decoding the other's deflate/gzip output — lives in the interop tests; this pins the TS side on
// its own. No golden bytes here: compressed bytes are not compared (PROTOCOL.md §6.1), only decode-to-input.
import {deflateCodec, gzipCodec, noOpCodec} from "../src/compression";
import {ProtocolEncoder} from "../src/ProtocolEncoder";
import {Header, Layout, Version, Width} from "../src/models";
import {ViPaqFormatError} from "../src/utils";
import {bin, item} from "./support/builders";

describe("compression codecs", () => {
	describe.each([
		{name: "no-op", codec: noOpCodec},
		{name: "deflate", codec: deflateCodec},
		{name: "gzip", codec: gzipCodec},
	])("$name", ({codec}) => {
		test("round-trips a body back to the original bytes", async () => {
			const body = Uint8Array.from({length: 300}, (_unused, index) => index % 7);
			const back = await codec.decompress(await codec.compress(body));
			expect(Array.from(back)).toEqual(Array.from(body));
		});
	});

	test("deflate shrinks repetitive data", async () => {
		const repetitive = new Uint8Array(500).fill(9);
		expect((await deflateCodec.compress(repetitive)).length).toBeLessThan(repetitive.length);
	});

	// Raw DEFLATE has no wrapper; gzip adds ~18 bytes of magic, mtime, OS byte and a CRC trailer. So on the same
	// input deflate is always the smaller of the two — which is why deflate is the pick.
	test("deflate output is smaller than gzip's (no wrapper)", async () => {
		const data = new Uint8Array(300).fill(4);
		expect((await deflateCodec.compress(data)).length).toBeLessThan((await gzipCodec.compress(data)).length);
	});

	describe.each([
		{name: "deflate", codec: deflateCodec},
		{name: "gzip", codec: gzipCodec},
	])("$name rejects garbage", ({codec}) => {
		test("throws ViPaqFormatError on bytes that are not its stream", async () => {
			await expect(codec.decompress(new Uint8Array([0xff, 0xff, 0xff]))).rejects.toBeInstanceOf(ViPaqFormatError);
		});
	});
});

describe("ProtocolEncoder compressed path", () => {
	const theBin = bin(20, 20, 20);
	const items = [item(1, 2, 3, 4, 5, 6), item(7, 8, 9, 10, 11, 12)];

	function header(compressed: boolean): Header {
		return new Header(Version.Version1, compressed, Layout.RowMajor, Width.Eight, Width.Eight, Width.Eight);
	}

	test.each([
		{name: "no-op", codec: noOpCodec},
		{name: "deflate", codec: deflateCodec},
		{name: "gzip", codec: gzipCodec},
	])("$name round-trips through a compressed blob", async ({codec}) => {
		const encoder = new ProtocolEncoder(codec);
		const blob = await encoder.encode(header(true), theBin, items);
		const decoded = await encoder.decode(header(true), blob.slice(Header.byteCount));

		expect(decoded.bin).toEqual(theBin);
		expect(decoded.items).toEqual(items);
	});

	// A real codec must actually shrink a compressible body — proves the compressed bit does something, not just
	// that NoOp round-trips. Many identical items give deflate plenty to grip.
	test("deflate makes a compressible blob smaller than the raw one", async () => {
		const many = Array.from({length: 300}, () => item(1, 2, 3, 4, 5, 6));
		const raw = await new ProtocolEncoder(noOpCodec).encode(header(false), theBin, many);
		const deflated = await new ProtocolEncoder(deflateCodec).encode(header(true), theBin, many);
		expect(deflated.length).toBeLessThan(raw.length);
	});

	// With NoOp the compressed path runs but the body stays readable, so a NoOp "compressed" blob has the same
	// body as the raw one — only the header's compressed bit differs. This is what makes the framing checkable.
	test("noOp leaves the body identical to the raw blob's", async () => {
		const raw = await new ProtocolEncoder(noOpCodec).encode(header(false), theBin, items);
		const noOpCompressed = await new ProtocolEncoder(noOpCodec).encode(header(true), theBin, items);
		expect(Array.from(noOpCompressed.slice(Header.byteCount))).toEqual(Array.from(raw.slice(Header.byteCount)));
	});
});
