// mirrors src/utils/getBodyLength.ts
// ports C#: Header.GetBodyLength. The body length the writer pre-sizes its buffer to: everything after the two
// header bytes = count(2) + 3*binBytes + count*(3*itemDimBytes + 3*itemCoordBytes). The header bytes are NOT
// counted here — ProtocolEncoder prepends them. If this is short the writer runs off the end.
import {getBodyLength} from "../../src/utils";
import {Header, Layout, Version, Width} from "../../src/models";

function header(binWidth: Width, itemDims: Width, itemCoords: Width): Header {
	return new Header(Version.Version1, false, Layout.RowMajor, binWidth, itemDims, itemCoords);
}

describe("getBodyLength", () => {
	test.each([
		{name: "all 8-bit, 1 item", header: header(Width.Eight, Width.Eight, Width.Eight), count: 1, size: 11},
		{name: "16-bit bin, 8-bit items, 1 item", header: header(Width.Sixteen, Width.Eight, Width.Eight), count: 1, size: 14},
		{name: "8-bit bin, 16-bit item dims, 1 item", header: header(Width.Eight, Width.Sixteen, Width.Eight), count: 1, size: 14},
		{name: "all 8-bit, 2 items", header: header(Width.Eight, Width.Eight, Width.Eight), count: 2, size: 17},
		{name: "all 8-bit, 0 items", header: header(Width.Eight, Width.Eight, Width.Eight), count: 0, size: 5},
	])("computes the size for $name", ({header: h, count, size}) => {
		expect(getBodyLength(h, count)).toBe(size);
	});
});
