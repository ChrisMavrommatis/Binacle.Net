// ports C#: LayoutCodecTests
//
// Drives both layout codecs through ProtocolEncoder and proves three things: each layout round-trips, the two
// differ on the wire but agree on length, and an unknown layout code is rejected. All uncompressed.
import {ProtocolEncoder} from "../src/ProtocolEncoder";
import {noOpCodec} from "../src/compression";
import {getLayoutDecoder, getLayoutEncoder} from "../src/layouts";
import {Header, Layout, Version, Width} from "../src/models";
import {bin, item} from "./support/builders";

const theBin = bin(20, 20, 20);
// Two items whose fields are all distinct, so row-major and columnar orderings produce different bytes.
const items = [item(1, 2, 3, 4, 5, 6), item(7, 8, 9, 10, 11, 12)];

function header(layout: Layout): Header {
	return new Header(Version.Version1, false, layout, Width.Eight, Width.Eight, Width.Eight);
}

describe("layout codecs", () => {
	test.each([
		{name: "row-major", layout: Layout.RowMajor},
		{name: "columnar", layout: Layout.Columnar},
	])("$name round-trips", async ({layout}) => {
		const encoder = new ProtocolEncoder(noOpCodec);
		const data = await encoder.encode(header(layout), theBin, items);
		const decoded = await encoder.decode(header(layout), data.slice(Header.byteCount));

		expect(decoded.bin).toEqual(theBin);
		expect(decoded.items).toEqual(items);
	});

	// The layout bit really reorders the body: same values, same length, different bytes.
	test("the two layouts differ on the wire but agree on length", async () => {
		const encoder = new ProtocolEncoder(noOpCodec);
		const rowMajor = await encoder.encode(header(Layout.RowMajor), theBin, items);
		const columnar = await encoder.encode(header(Layout.Columnar), theBin, items);

		const rowMajorBody = Array.from(rowMajor.slice(Header.byteCount));
		const columnarBody = Array.from(columnar.slice(Header.byteCount));

		expect(columnarBody.length).toBe(rowMajorBody.length);
		expect(columnarBody).not.toEqual(rowMajorBody);
	});

	test("an unknown layout is rejected", () => {
		expect(() => getLayoutEncoder(99 as Layout)).toThrow();
		expect(() => getLayoutDecoder(99 as Layout)).toThrow();
	});
});
