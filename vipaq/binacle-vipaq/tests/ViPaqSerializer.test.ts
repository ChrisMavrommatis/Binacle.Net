// mirrors src/ViPaqSerializer.ts (serialize + deserialize tested together — they are inverses)
import ViPaqSerializer from "../src/ViPaqSerializer";
import {Dimensions, Version} from "../src/models";
import {encodingInfoFromByte} from "../src/utils";
import {Item, anItem, bin, item} from "./support/builders";
import {expectBytes} from "./support/bytes";
import {exactBytesCases} from "./providers/ExactBytes";

async function roundTrip(input: Dimensions, items: Item[]) {
	const data = await ViPaqSerializer.serialize(input, items);
	return {data, result: await ViPaqSerializer.deserialize(data)};
}

describe("ViPaqSerializer", () => {
	describe("serialize produces exact bytes", () => {
		// ports C#: SerializationEncodingTests.Serialize_Produces_Exact_Bytes_For_Known_Input
		test.each(exactBytesCases)("for $name", async ({bin: input, items, bytes}) => {
			const data = await ViPaqSerializer.serialize(input, items);
			expectBytes(data, bytes);
		});

		// ports C#: Serialize_Writes_Correct_BinDimensionsBitSize
		test.each([
			{name: "255 -> 8-bit", dimension: 255, bitSize: 0},
			{name: "256 -> 16-bit", dimension: 256, bitSize: 1},
			{name: "65536 -> 32-bit", dimension: 65536, bitSize: 2},
		])("chooses the bin width: $name", async ({dimension, bitSize}) => {
			const data = await ViPaqSerializer.serialize(bin(dimension, dimension, dimension), [item(1, 1, 1, 1, 1, 1)]);
			expect(encodingInfoFromByte(data[0]).binDimensionsBitSize).toBe(bitSize);
		});
	});

	describe("round trips", () => {
		// ports C#: SerializationRoundTripTests.Int32_RoundTrips_*_Bit
		test.each([
			{name: "8-bit values", input: bin(100, 110, 120), items: [item(10, 20, 30, 1, 2, 3), item(40, 50, 60, 10, 20, 30)]},
			{name: "16-bit values", input: bin(1000, 2000, 3000), items: [item(300, 400, 500, 600, 700, 800)]},
			{name: "32-bit values", input: bin(100_000, 200_000, 300_000), items: [item(70_000, 80_000, 90_000, 100_000, 110_000, 120_000)]},
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

		// ports C#: RoundTrips_When_Body_Is_Compressed
		test("a compressed body", async () => {
			const items = Array.from({length: 60}, () => item(10, 20, 30, 1, 2, 3));
			const {result} = await roundTrip(bin(200, 200, 200), items);
			expect(result.items).toHaveLength(60);
		});
	});

	describe("compression flag", () => {
		// ports C#: Serialize_Marks_Version_Uncompressed_When_Body_Small
		test("uncompressed when the body is small", async () => {
			const data = await ViPaqSerializer.serialize(bin(10, 20, 30), [item(1, 2, 3, 4, 5, 6)]);
			expect(encodingInfoFromByte(data[0]).version).toBe(Version.Uncompressed);
		});

		// ports C#: Serialize_Marks_Version_CompressedGzip_When_Body_Large
		test("compressed when the body is large", async () => {
			const items = Array.from({length: 60}, () => item(10, 20, 30, 1, 2, 3));
			const data = await ViPaqSerializer.serialize(bin(200, 200, 200), items);
			expect(encodingInfoFromByte(data[0]).version).toBe(Version.CompressedGzip);
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

		// ports C#: SerializationBehaviorTests (the negative-coordinate guard)
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

		// ports C#: Deserialize_Throws_NotSupportedException_When_Version_Is_Reserved
		test("on a reserved version", async () => {
			const data = new Uint8Array(exactBytesCases[0].bytes);
			data[0] = (data[0] & 0b00_11_11_11) | (Version.Reserved2 << 6);
			await expect(ViPaqSerializer.deserialize(data)).rejects.toThrow();
		});
	});
});
