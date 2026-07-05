// mirrors src/utils/getCoordinatesBitSize.ts
// Mirrors C# BitSizeHelper.GetCoordinatesBitSize, separate impl. Unlike dimensions, coordinate 0 is
// legal (an item flush against the origin) — uses < 0, not <= 0. Bug #2 lived here: it checked `width`
// instead of `z` and rejected 0.
import {getCoordinatesBitSize, Sizes} from "../../src/utils";
import {BitSize} from "../../src/models";
import {anItem} from "../support/builders";

describe("getCoordinatesBitSize", () => {
	describe("picks the smallest width that fits", () => {
		test.each([
			{name: "8-bit", coords: anItem({x: 0, y: 100, z: 255}), expected: BitSize.Eight},
			{name: "16-bit", coords: anItem({x: 256, y: 0, z: 65535}), expected: BitSize.Sixteen},
			{name: "32-bit", coords: anItem({x: 65536, y: 0, z: 0}), expected: BitSize.ThirtyTwo},
			{name: "64-bit", coords: anItem({x: 4294967296, y: 0, z: 0}), expected: BitSize.SixtyFour},
		])("$name", ({coords, expected}) => {
			expect(getCoordinatesBitSize(coords)).toBe(expected);
		});
	});

	// The bug #2 regression: zero is a valid coordinate and must not throw.
	test("allows a coordinate of zero", () => {
		expect(getCoordinatesBitSize(anItem({x: 0, y: 0, z: 0}))).toBe(BitSize.Eight);
	});

	describe("rejects negative coordinates", () => {
		test.each([
			{name: "x", coords: anItem({x: -1})},
			{name: "y", coords: anItem({y: -1})},
			{name: "z", coords: anItem({z: -1})},
		])("throws on negative $name", ({coords}) => {
			expect(() => getCoordinatesBitSize(coords)).toThrow();
		});
	});

	// The 64-bit bucket stops at maxInteger (2^53 - 1), not the full 64-bit range — see vipaq/PROTOCOL.md.
	describe("caps the 64-bit bucket at maxInteger", () => {
		test("the largest in-range value selects 64-bit", () => {
			expect(getCoordinatesBitSize(anItem({x: Sizes.maxInteger}))).toBe(BitSize.SixtyFour);
		});

		// A value above the ceiling throws and names the offending axis (matches C#'s per-field ParamName).
		// ports C#: GetCoordinatesBitSize_Throws_ArgumentOutOfRangeException_When_Value_Exceeds_MaxInteger
		test.each([
			{name: "x", coords: anItem({x: Sizes.maxInteger + 1})},
			{name: "y", coords: anItem({y: Sizes.maxInteger + 1})},
			{name: "z", coords: anItem({z: Sizes.maxInteger + 1})},
		])("a value above maxInteger throws naming $name", ({name, coords}) => {
			expect(() => getCoordinatesBitSize(coords)).toThrow(`'${name}' exceeds the max supported value`);
		});
	});
});
