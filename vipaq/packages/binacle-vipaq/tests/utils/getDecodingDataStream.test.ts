// mirrors src/utils/getDecodingDataStream.ts
// TS-specific: returns a DataView over the body, decompressing first only when the header says
// CompressedGzip. Reserved versions (2/3) are rejected here — this is TS's equivalent of C#'s
// reserved-version NotSupportedException, on a different code path.
import {getDecodingDataStream, compressBuffer} from "../../src/utils";
import {BitSize, EncodingInfo, Version} from "../../src/models";

const eightBit = (version: Version) => new EncodingInfo(version, BitSize.Eight, BitSize.Eight, BitSize.Eight);

describe("getDecodingDataStream", () => {
	test("uncompressed: returns a view over the same bytes", async () => {
		const body = new Uint8Array([0x0a, 0x14, 0x1e]);

		const view = await getDecodingDataStream(body, eightBit(Version.Uncompressed));

		expect([view.getUint8(0), view.getUint8(1), view.getUint8(2)]).toEqual([0x0a, 0x14, 0x1e]);
	});

	test("compressed: decompresses back to the original body", async () => {
		const body = new Uint8Array(Array.from({length: 300}, (_, i) => i % 256));
		const compressed = await compressBuffer(body);

		const view = await getDecodingDataStream(compressed, eightBit(Version.CompressedGzip));

		const restored = Array.from({length: body.length}, (_, i) => view.getUint8(i));
		expect(restored).toEqual(Array.from(body));
	});

	test("throws on a reserved version", async () => {
		await expect(
			getDecodingDataStream(new Uint8Array([1, 2, 3]), eightBit(Version.Reserved2)),
		).rejects.toThrow();
	});
});
