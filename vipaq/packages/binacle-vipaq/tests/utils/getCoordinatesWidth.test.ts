// mirrors src/utils/getCoordinatesWidth.ts
// Mirrors C# WidthHelper.GetCoordinatesWidth, separate impl. Unlike dimensions, coordinate 0 is legal (an item
// flush against the origin), so the guard is < 0, not <= 0.
import {getCoordinatesWidth, Sizes} from "../../src/utils";
import {Width} from "../../src/models";
import {anItem} from "../support/builders";

describe("getCoordinatesWidth", () => {
	describe("picks the smallest width that fits", () => {
		test.each([
			{name: "8-bit", coords: anItem({x: 0, y: 100, z: 255}), expected: Width.Eight},
			{name: "16-bit", coords: anItem({x: 256, y: 0, z: 65535}), expected: Width.Sixteen},
		])("$name", ({coords, expected}) => {
			expect(getCoordinatesWidth(coords)).toBe(expected);
		});
	});

	// Zero is a valid coordinate and must not throw.
	test("allows a coordinate of zero", () => {
		expect(getCoordinatesWidth(anItem({x: 0, y: 0, z: 0}))).toBe(Width.Eight);
	});

	describe("rejects negative coordinates", () => {
		test.each([
			{name: "x", coords: anItem({x: -1})},
			{name: "y", coords: anItem({y: -1})},
			{name: "z", coords: anItem({z: -1})},
		])("throws on negative $name", ({coords}) => {
			expect(() => getCoordinatesWidth(coords)).toThrow();
		});
	});

	describe("rejects values above the 16-bit ceiling", () => {
		test("the largest in-range value selects 16-bit", () => {
			expect(getCoordinatesWidth(anItem({x: Sizes.maxValue}))).toBe(Width.Sixteen);
		});

		// A value above the ceiling throws and names the offending axis (matches C#'s per-field ParamName).
		test.each([
			{name: "x", coords: anItem({x: Sizes.maxValue + 1})},
			{name: "y", coords: anItem({y: Sizes.maxValue + 1})},
			{name: "z", coords: anItem({z: Sizes.maxValue + 1})},
		])("a value above the ceiling throws naming $name", ({name, coords}) => {
			expect(() => getCoordinatesWidth(coords)).toThrow(`'${name}' exceeds the max supported value`);
		});
	});
});
