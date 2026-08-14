// mirrors src/utils/createHeader.ts
// Picks each section's width from the largest value in it. Mirrors C# Header.Create. Curated literals, no
// faker: one value per bucket.
import {createHeader} from "../../src/utils";
import {Width} from "../../src/models";
import {bin, item} from "../support/builders";

const buckets = [
	{name: "8-bit", value: 200, width: Width.Eight},
	{name: "16-bit", value: 5_000, width: Width.Sixteen},
];

describe("createHeader", () => {
	test.each(buckets)("sizes every section to $name when all values are $name", ({value, width}) => {
		const header = createHeader(bin(value, value, value), [item(value, value, value, value, value, value)]);
		expect(header.binDimensionsWidth).toBe(width);
		expect(header.itemDimensionsWidth).toBe(width);
		expect(header.itemCoordinatesWidth).toBe(width);
	});

	test.each(buckets)("sizes only the bin to $name, items stay 8-bit", ({value, width}) => {
		const header = createHeader(bin(value, value, value), [item(1, 1, 1, 1, 1, 1)]);
		expect(header.binDimensionsWidth).toBe(width);
		expect(header.itemDimensionsWidth).toBe(Width.Eight);
		expect(header.itemCoordinatesWidth).toBe(Width.Eight);
	});

	test.each(buckets)("sizes only the item dimensions to $name", ({value, width}) => {
		const header = createHeader(bin(1, 1, 1), [item(value, value, value, 1, 1, 1)]);
		expect(header.binDimensionsWidth).toBe(Width.Eight);
		expect(header.itemDimensionsWidth).toBe(width);
		expect(header.itemCoordinatesWidth).toBe(Width.Eight);
	});

	test.each(buckets)("sizes only the item coordinates to $name", ({value, width}) => {
		const header = createHeader(bin(1, 1, 1), [item(1, 1, 1, value, value, value)]);
		expect(header.binDimensionsWidth).toBe(Width.Eight);
		expect(header.itemDimensionsWidth).toBe(Width.Eight);
		expect(header.itemCoordinatesWidth).toBe(width);
	});

	// ports C#: Header.Create uses the largest item across the list
	test("uses the largest item across the list", () => {
		const header = createHeader(bin(1, 1, 1), [item(1, 1, 1, 1, 1, 1), item(5_000, 1, 1, 1, 1, 1)]);
		expect(header.itemDimensionsWidth).toBe(Width.Sixteen);
	});

	// ports C#: with no items, both item widths stay Eight (PROTOCOL.md §4)
	test("defaults item widths to 8-bit when there are no items", () => {
		const header = createHeader(bin(5_000, 5_000, 5_000), []);
		expect(header.itemDimensionsWidth).toBe(Width.Eight);
		expect(header.itemCoordinatesWidth).toBe(Width.Eight);
	});

	// ports C#: ValidationHelper.ThrowIfTooManyItems. The count rides in a uint16, so 65535 is the ceiling.
	// The guard only counts, so identical 1x1x1 items are enough.
	describe("enforces the item-count limit", () => {
		test("accepts 65535 items", () => {
			const items = Array.from({length: 65535}, () => item(1, 1, 1, 0, 0, 0));
			expect(() => createHeader(bin(1, 1, 1), items)).not.toThrow();
		});

		test("throws on 65536 items", () => {
			const items = Array.from({length: 65536}, () => item(1, 1, 1, 0, 0, 0));
			expect(() => createHeader(bin(1, 1, 1), items)).toThrow();
		});
	});
});
