// mirrors src/ViPaqSerializer.ts (serialize + deserialize tested together — they are inverses)
// ports C#: ViPaqSerializerTests + SerializationEncodingTests + SerializationBehaviorTests
//
// ViPaqSerializer is the choosing layer: by default it writes raw, row-major, narrowest. Compression and layout
// are opt-in options (defaults off / row-major). So these cover its own job — exact bytes for the default,
// the width it chooses, a real end-to-end round trip, the opt-in options, and the guards. The forced wider/mode
// variants live in roundTrip.test.ts (they go through ProtocolEncoder directly).
import ViPaqSerializer from "../src/ViPaqSerializer";
import {Dimensions, Layout, Version, Width} from "../src/models";
import {headerFromBytes} from "../src/utils";
import {Item, anItem, bin, item} from "./support/builders";
import {expectBytes} from "./support/bytes";
import {exactBytesCases} from "./providers/ExactBytes";

async function roundTrip(input: Dimensions, items: Item[]) {
	const data = await ViPaqSerializer.serialize(input, items);
	return {data, result: await ViPaqSerializer.deserialize(data)};
}

describe("ViPaqSerializer", () => {
	describe("serialize produces exact bytes", () => {
		// ports C#: SerializationEncodingTests.Encode_Produces_Exact_Bytes (all exact-bytes vectors are raw,
		// row-major, narrowest — exactly what ViPaqSerializer chooses, so its output is the golden).
		test.each(exactBytesCases)("for $name", async ({bin: input, items, bytes}) => {
			const data = await ViPaqSerializer.serialize(input, items);
			expectBytes(data, bytes);
		});

		// ports C#: ViPaqSerializer_Chooses_Correct_Widths_In_Header. Only 8- and 16-bit widths exist; a value
		// above 65535 is rejected (see encodeInvalid), not widened.
		test.each([
			{name: "255 -> 8-bit", dimension: 255, width: Width.Eight},
			{name: "256 -> 16-bit", dimension: 256, width: Width.Sixteen},
			{name: "65535 -> 16-bit", dimension: 65535, width: Width.Sixteen},
		])("chooses the bin width: $name", async ({dimension, width}) => {
			const data = await ViPaqSerializer.serialize(bin(dimension, dimension, dimension), [item(1, 1, 1, 1, 1, 1)]);
			expect(headerFromBytes(data[0], data[1]).binDimensionsWidth).toBe(width);
		});
	});

	describe("round trips through the public API", () => {
		// ports C#: ViPaqSerializerTests.Round_Trips_*_Through_The_Public_Api
		test.each([
			{name: "8-bit values", input: bin(100, 110, 120), items: [item(10, 20, 30, 1, 2, 3), item(40, 50, 60, 10, 20, 30)]},
			{name: "16-bit values", input: bin(1000, 2000, 3000), items: [item(300, 400, 500, 600, 700, 800)]},
		])("$name", async ({input, items}) => {
			const {result} = await roundTrip(input, items);
			expect(result.bin).toEqual(input);
			expect(result.items).toEqual(items);
		});

		// ports C#: RoundTrips_When_Coordinates_Are_Zero
		test("zero coordinates survive", async () => {
			const {result} = await roundTrip(bin(100, 110, 120), [item(10, 20, 30, 0, 0, 0)]);
			expect(result.items[0]).toEqual(item(10, 20, 30, 0, 0, 0));
		});

		// ports C#: RoundTrips_When_There_Are_No_Items
		test("no items, count comes back as 0", async () => {
			const {result} = await roundTrip(bin(100, 110, 120), []);
			expect(result.items).toHaveLength(0);
			expect(result.bin).toEqual(bin(100, 110, 120));
		});

		// ports C#: RoundTrips_When_Item_Count_Exceeds_255 (the count rides in a uint16)
		test("many items, a uint16 count", async () => {
			const items = Array.from({length: 300}, (_, i) => item(1, 2, 3, i % 256, 5, 6));
			const {result} = await roundTrip(bin(10, 20, 30), items);
			expect(result.items).toHaveLength(300);
		});
	});

	describe("serialize throws", () => {
		// ports C#: Serialize_Throws_When_Bin_Is_Null
		test("on a null bin", async () => {
			await expect(
				ViPaqSerializer.serialize(null as unknown as Dimensions, [item(1, 2, 3, 4, 5, 6)]),
			).rejects.toThrow();
		});

		// ports C#: Serialize_Throws_When_Items_Are_Null
		test("on null items", async () => {
			await expect(ViPaqSerializer.serialize(bin(10, 20, 30), null as unknown as Item[])).rejects.toThrow();
		});

		// ports C#: the negative-coordinate guard
		test("on a negative coordinate", async () => {
			await expect(ViPaqSerializer.serialize(bin(10, 20, 30), [anItem({z: -1})])).rejects.toThrow();
		});
	});

	describe("deserialize throws", () => {
		// ports C#: Deserialize_Throws_When_Data_Is_Null / _Empty
		test("on null or empty data", async () => {
			await expect(ViPaqSerializer.deserialize(null as unknown as Uint8Array<ArrayBuffer>)).rejects.toThrow();
			await expect(ViPaqSerializer.deserialize(new Uint8Array(0))).rejects.toThrow();
		});

		// ports C#: Deserialize_Throws_When_Version_Is_Reserved
		test("on a reserved version", async () => {
			const data = new Uint8Array(exactBytesCases[0].bytes);
			data[0] = (data[0] & 0b0011_1111) | (Version.Reserved2 << 6);
			await expect(ViPaqSerializer.deserialize(data)).rejects.toThrow();
		});

		// A blob whose compressed bit is set but whose body is not a valid DEFLATE stream is malformed: decode
		// runs the body through the codec and the codec rejects it. (Here the body is a valid *raw* blob, which is
		// not valid deflate.)
		test("rejects a compressed blob with a body that is not a deflate stream", async () => {
			const data = new Uint8Array(exactBytesCases[0].bytes);
			data[0] = data[0] | 0b0010_0000;
			await expect(ViPaqSerializer.deserialize(data)).rejects.toThrow();
		});
	});

	// ports C#: SerializationOptionsTests. The opt-in paths — compression and columnar layout.
	describe("options", () => {
		const compressibleBin = bin(1000, 1000, 1000);
		const repetitiveItems = (count: number) =>
			Array.from({length: count}, () => item(300, 300, 300, 0, 0, 0));

		// every combination decodes back to the input — decode-to-input is the oracle (§6.1)
		test.each([
			{compress: false, layout: Layout.RowMajor},
			{compress: false, layout: Layout.Columnar},
			{compress: true, layout: Layout.RowMajor},
			{compress: true, layout: Layout.Columnar},
		])("round-trips with compress=$compress layout=$layout", async ({compress, layout}) => {
			const items = repetitiveItems(50);
			const data = await ViPaqSerializer.serialize(compressibleBin, items, {compress, layout});
			const result = await ViPaqSerializer.deserialize(data);

			expect(result.bin).toEqual(compressibleBin);
			expect(result.items).toEqual(items);
		});

		test("columnar sets the layout bit", async () => {
			const data = await ViPaqSerializer.serialize(compressibleBin, repetitiveItems(4), {layout: Layout.Columnar});
			expect(headerFromBytes(data[0], data[1]).layout).toBe(Layout.Columnar);
		});

		test("compresses a large repetitive pack", async () => {
			const items = repetitiveItems(50);
			const raw = await ViPaqSerializer.serialize(compressibleBin, items);
			const compressed = await ViPaqSerializer.serialize(compressibleBin, items, {compress: true});

			expect(compressed.length).toBeLessThan(raw.length);
			expect(headerFromBytes(compressed[0], compressed[1]).compressed).toBe(true);
		});
	});
});
