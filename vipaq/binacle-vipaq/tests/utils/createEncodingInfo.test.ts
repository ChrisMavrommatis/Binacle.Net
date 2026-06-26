// mirrors src/utils/createEncodingInfo.ts
// Picks each section's width from the largest value in it: bin dims set the bin width; the largest item
// dims across all items set the item-dims width; likewise item coords. Mirrors C# CreateEncodingInfo.
// Curated literals, no faker — one representative value per bucket.
import {createEncodingInfo} from "../../src/utils";
import {BitSize} from "../../src/models";
import {bin, item} from "../support/builders";

const buckets = [
	{name: "8-bit", value: 200, bitSize: BitSize.Eight},
	{name: "16-bit", value: 5_000, bitSize: BitSize.Sixteen},
	{name: "32-bit", value: 100_000, bitSize: BitSize.ThirtyTwo},
	{name: "64-bit", value: 5_000_000_000, bitSize: BitSize.SixtyFour},
];

describe("createEncodingInfo", () => {
	test.each(buckets)("sizes every section to $name when all values are $name", ({value, bitSize}) => {
		const info = createEncodingInfo(bin(value, value, value), [item(value, value, value, value, value, value)]);
		expect(info.binDimensionsBitSize).toBe(bitSize);
		expect(info.itemDimensionsBitSize).toBe(bitSize);
		expect(info.itemCoordinatesBitSize).toBe(bitSize);
	});

	test.each(buckets)("sizes only the bin to $name, items stay 8-bit", ({value, bitSize}) => {
		const info = createEncodingInfo(bin(value, value, value), [item(1, 1, 1, 1, 1, 1)]);
		expect(info.binDimensionsBitSize).toBe(bitSize);
		expect(info.itemDimensionsBitSize).toBe(BitSize.Eight);
		expect(info.itemCoordinatesBitSize).toBe(BitSize.Eight);
	});

	test.each(buckets)("sizes only the item dimensions to $name", ({value, bitSize}) => {
		const info = createEncodingInfo(bin(1, 1, 1), [item(value, value, value, 1, 1, 1)]);
		expect(info.binDimensionsBitSize).toBe(BitSize.Eight);
		expect(info.itemDimensionsBitSize).toBe(bitSize);
		expect(info.itemCoordinatesBitSize).toBe(BitSize.Eight);
	});

	test.each(buckets)("sizes only the item coordinates to $name", ({value, bitSize}) => {
		const info = createEncodingInfo(bin(1, 1, 1), [item(1, 1, 1, value, value, value)]);
		expect(info.binDimensionsBitSize).toBe(BitSize.Eight);
		expect(info.itemDimensionsBitSize).toBe(BitSize.Eight);
		expect(info.itemCoordinatesBitSize).toBe(bitSize);
	});

	// ports C#: CreateEncodingInfo_Uses_Largest_Item_When_Items_Differ
	test("uses the largest item across the list", () => {
		const info = createEncodingInfo(bin(1, 1, 1), [item(1, 1, 1, 1, 1, 1), item(5_000, 1, 1, 1, 1, 1)]);
		expect(info.itemDimensionsBitSize).toBe(BitSize.Sixteen);
	});

	// ports C#: CreateEncodingInfo_Defaults_Item_Sizes_To_Eight_When_No_Items
	test("defaults item sizes to 8-bit when there are no items", () => {
		const info = createEncodingInfo(bin(5_000, 5_000, 5_000), []);
		expect(info.itemDimensionsBitSize).toBe(BitSize.Eight);
		expect(info.itemCoordinatesBitSize).toBe(BitSize.Eight);
	});
});
