// mirrors src/utils/getCoordinatesBitSize.ts
// Mirrors C# BitSizeHelper.GetCoordinatesBitSize, separate impl. Unlike dimensions, coordinate 0 is
// legal (an item flush against the origin) — uses < 0, not <= 0. Bug #2 lived here: it checked `width`
// instead of `z` and rejected 0.
import {getCoordinatesBitSize} from "../../src/utils";
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
});
