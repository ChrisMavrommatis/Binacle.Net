// mirrors src/utils/getDimensionsBitSize.ts
// Mirrors C# BitSizeHelper.GetDimensionsBitSize, separate impl. Dimensions must be positive — uses
// <= 0, so 0 is rejected (unlike coordinates).
import {getDimensionsBitSize, Sizes} from "../../src/utils";
import {BitSize} from "../../src/models";
import {bin} from "../support/builders";

describe("getDimensionsBitSize", () => {
	describe("picks the smallest width that fits", () => {
		test.each([
			{name: "8-bit", dims: bin(1, 100, 255), expected: BitSize.Eight},
			{name: "16-bit", dims: bin(256, 1, 65535), expected: BitSize.Sixteen},
			{name: "32-bit", dims: bin(65536, 1, 1), expected: BitSize.ThirtyTwo},
			{name: "64-bit", dims: bin(4294967296, 1, 1), expected: BitSize.SixtyFour},
		])("$name", ({dims, expected}) => {
			expect(getDimensionsBitSize(dims)).toBe(expected);
		});
	});

	describe("rejects non-positive dimensions", () => {
		test.each([
			{name: "length 0", dims: bin(0, 1, 1)},
			{name: "width 0", dims: bin(1, 0, 1)},
			{name: "height 0", dims: bin(1, 1, 0)},
			{name: "negative length", dims: bin(-1, 1, 1)},
		])("throws on $name", ({dims}) => {
			expect(() => getDimensionsBitSize(dims)).toThrow();
		});
	});

	// The 64-bit bucket stops at maxInteger (2^53 - 1), not the full 64-bit range — see vipaq/PROTOCOL.md.
	describe("caps the 64-bit bucket at maxInteger", () => {
		test("the largest in-range value selects 64-bit", () => {
			expect(getDimensionsBitSize(bin(Sizes.maxInteger, 1, 1))).toBe(BitSize.SixtyFour);
		});

		test("a value above maxInteger throws", () => {
			expect(() => getDimensionsBitSize(bin(Sizes.maxInteger + 1, 1, 1))).toThrow();
		});
	});
});
