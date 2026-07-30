// mirrors src/utils/getDimensionsWidth.ts
// Mirrors C# WidthHelper.GetDimensionsWidth, separate impl. Dimensions must be positive — uses <= 0, so 0 is
// rejected (unlike coordinates). Only Eight and Sixteen exist; a value above the 16-bit ceiling is rejected
// outright (no wider tier, no saturation).
import {getDimensionsWidth, Sizes} from "../../src/utils";
import {Width} from "../../src/models";
import {bin} from "../support/builders";

describe("getDimensionsWidth", () => {
	describe("picks the smallest width that fits", () => {
		test.each([
			{name: "8-bit", dims: bin(1, 100, 255), expected: Width.Eight},
			{name: "16-bit", dims: bin(256, 1, 65535), expected: Width.Sixteen},
		])("$name", ({dims, expected}) => {
			expect(getDimensionsWidth(dims)).toBe(expected);
		});
	});

	describe("rejects non-positive dimensions", () => {
		test.each([
			{name: "length 0", dims: bin(0, 1, 1)},
			{name: "width 0", dims: bin(1, 0, 1)},
			{name: "height 0", dims: bin(1, 1, 0)},
			{name: "negative length", dims: bin(-1, 1, 1)},
		])("throws on $name", ({dims}) => {
			expect(() => getDimensionsWidth(dims)).toThrow();
		});
	});

	describe("rejects values above the 16-bit ceiling", () => {
		test("the largest in-range value selects 16-bit", () => {
			expect(getDimensionsWidth(bin(Sizes.maxValue, 1, 1))).toBe(Width.Sixteen);
		});

		// A value above the ceiling throws and names the offending field (matches C#'s per-field ParamName).
		test.each([
			{name: "length", dims: bin(Sizes.maxValue + 1, 1, 1)},
			{name: "width", dims: bin(1, Sizes.maxValue + 1, 1)},
			{name: "height", dims: bin(1, 1, Sizes.maxValue + 1)},
		])("a value above the ceiling throws naming $name", ({name, dims}) => {
			expect(() => getDimensionsWidth(dims)).toThrow(`'${name}' exceeds the max supported value`);
		});
	});
});
