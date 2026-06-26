// mirrors src/utils/getBufferSize.ts
// TS-specific: pre-computes the exact Uint8Array size before writing. Total =
// header(3) + 3*binBytes + count*(3*itemDimBytes + 3*itemCoordBytes). If it is short the writer runs
// off the end — the failure mode of bug #1.
import {getBufferSize} from "../../src/utils";
import {BitSize, EncodingInfo, Version} from "../../src/models";

function info(bin: BitSize, itemDims: BitSize, itemCoords: BitSize): EncodingInfo {
	return new EncodingInfo(Version.Uncompressed, bin, itemDims, itemCoords);
}

describe("getBufferSize", () => {
	test.each([
		{name: "all 8-bit, 1 item", encodingInfo: info(BitSize.Eight, BitSize.Eight, BitSize.Eight), count: 1, size: 12},
		{name: "16-bit bin, 8-bit items, 1 item", encodingInfo: info(BitSize.Sixteen, BitSize.Eight, BitSize.Eight), count: 1, size: 15},
		{name: "all 8-bit, 2 items", encodingInfo: info(BitSize.Eight, BitSize.Eight, BitSize.Eight), count: 2, size: 18},
		{name: "32-bit bin, 8-bit items, 1 item", encodingInfo: info(BitSize.ThirtyTwo, BitSize.Eight, BitSize.Eight), count: 1, size: 21},
		{name: "all 8-bit, 0 items", encodingInfo: info(BitSize.Eight, BitSize.Eight, BitSize.Eight), count: 0, size: 6},
	])("computes the size for $name", ({encodingInfo, count, size}) => {
		expect(getBufferSize(encodingInfo, count)).toBe(size);
	});
});
