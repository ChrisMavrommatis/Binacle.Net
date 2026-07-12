// mirrors src/ViPaqSerializer.ts (serialize + deserialize tested together — they are inverses)
// ports C#: ViPaqSerializerTests + SerializationEncodingTests + SerializationBehaviorTests
//
// ViPaqSerializer is the choosing layer: it always writes raw, row-major, narrowest, and refuses to read a
// compressed blob. So these cover its own job — exact bytes for known input, the width it chooses, a real
// end-to-end round trip through the public API, and the guards. The columnar/wider variants live in
// roundTrip.test.ts (they go through ProtocolEncoder, which ViPaqSerializer never can).
//
// Folded away from the old suite: the "compression flag" section is gone — ViPaqSerializer never compresses now
// (compression is deferred, PROTOCOL.md §6). Its inverse, that a compressed blob is refused on decode, is pinned
// by "rejects a compressed blob" below. The old 32/64-bit round-trip rows are gone with those tiers.
import ViPaqSerializer from "../src/ViPaqSerializer";
import {Dimensions, Version, Width} from "../src/models";
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

		// ports C#: Deserialize_Throws_NotSupported_When_Blob_Is_Compressed. The compressed bit is byte 0 bit 5;
		// the body is otherwise a valid raw blob, so only the compressed flag can be the cause.
		test("rejects a compressed blob", async () => {
			const data = new Uint8Array(exactBytesCases[0].bytes);
			data[0] = data[0] | 0b0010_0000;
			await expect(ViPaqSerializer.deserialize(data)).rejects.toThrow();
		});
	});
});
