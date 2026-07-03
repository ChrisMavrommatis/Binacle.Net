// Mirrors the C# Binacle.CompactNotation.UnitTests. Same grammar, same reject cases, so the two libraries
// can't drift.

import {
	detect,
	format,
	formatCoordinates,
	formatDimensions,
	parseCoordinates,
	parseDimensions,
	parseItem,
	parseItems,
	parseQuantity,
} from "../src";

describe("parse dimensions", () => {
	test.each([
		["10x20x30", 10, 20, 30],
		[" 10x20x30 ", 10, 20, 30],
		["-5x3x2", -5, 3, 2], // '-' is free now, so negatives parse
		["0x0x0", 0, 0, 0],
	])("'%s' reads three values split on x", (compact, length, width, height) => {
		expect(parseDimensions(compact as string)).toEqual({length, width, height});
	});

	test.each(["10x20", "10x20x30x40", "(1,2,3)"])("rejects '%s'", (compact) => {
		expect(() => parseDimensions(compact)).toThrow();
	});
});

describe("parse coordinates", () => {
	test.each([
		["(1,2,3)", 1, 2, 3],
		[" (1,2,3) ", 1, 2, 3],
		["(-1,-2,-3)", -1, -2, -3],
	])("'%s' reads three values inside parens", (compact, x, y, z) => {
		expect(parseCoordinates(compact as string)).toEqual({x, y, z});
	});

	test.each(["1,2,3", "(1,2)", "10x20x30"])("rejects '%s'", (compact) => {
		expect(() => parseCoordinates(compact)).toThrow();
	});
});

describe("parse quantity", () => {
	test.each([
		["[5]", 5],
		[" [12] ", 12],
	])("'%s' reads the int inside brackets", (compact, expected) => {
		expect(parseQuantity(compact as string)).toBe(expected);
	});

	test.each(["5", "[abc]"])("rejects '%s'", (compact) => {
		expect(() => parseQuantity(compact)).toThrow();
	});
});

describe("parse item", () => {
	test("reads dimensions and coordinates", () => {
		expect(parseItem("10x20x30 (1,2,3)")).toEqual({length: 10, width: 20, height: 30, x: 1, y: 2, z: 3});
	});

	test("rejects a quantity suffix", () => {
		expect(() => parseItem("10x20x30 (1,2,3) [3]")).toThrow();
	});

	test("rejects a missing coordinate block", () => {
		expect(() => parseItem("10x20x30")).toThrow();
	});
});

describe("parse items", () => {
	test("without a quantity returns one item", () => {
		expect(parseItems("10x20x30 (1,2,3)")).toHaveLength(1);
	});

	test("expands the quantity into that many copies", () => {
		const items = parseItems("10x20x30 (1,2,3) [3]");
		expect(items).toHaveLength(3);
		expect(items.every((item) => item.length === 10 && item.x === 1)).toBe(true);
	});

	test("returns distinct instances", () => {
		const items = parseItems("1x1x1 (0,0,0) [2]");
		expect(items[0]).not.toBe(items[1]);
	});

	test("flattens many strings", () => {
		expect(parseItems(["1x1x1 (0,0,0) [2]", "2x2x2 (1,1,1)"])).toHaveLength(3);
	});
});

describe("format", () => {
	test("formatDimensions writes LxWxH", () => {
		expect(formatDimensions({length: 10, width: 20, height: 30})).toBe("10x20x30");
	});

	test("formatCoordinates writes parens", () => {
		expect(formatCoordinates({x: 1, y: 2, z: 3})).toBe("(1,2,3)");
	});

	test("a dimensions-only object writes one block", () => {
		expect(format({length: 10, width: 20, height: 30})).toBe("10x20x30");
	});

	test("an item writes dimensions then coordinates", () => {
		expect(format({length: 10, width: 20, height: 30, x: 1, y: 2, z: 3})).toBe("10x20x30 (1,2,3)");
	});

	test("appends every block the object carries", () => {
		expect(format({length: 10, width: 20, height: 30, x: 1, y: 2, z: 3, quantity: 5})).toBe("10x20x30 (1,2,3) [5]");
	});

	test("rejects an object with no block", () => {
		expect(() => format({})).toThrow();
	});
});

describe("detect", () => {
	test.each([
		["(1,2,3)", "coordinates"],
		["[5]", "quantity"],
		["10x20x30", "dimensions"],
		[" (1,2,3)", "coordinates"],
	])("'%s' -> %s", (compact, kind) => {
		expect(detect(compact)).toBe(kind);
	});

	test("rejects an unknown string", () => {
		expect(() => detect("nonsense")).toThrow();
	});
});

describe("round-trip", () => {
	test.each(["10x20x30", "1x1x1"])("dimensions '%s'", (compact) => {
		expect(formatDimensions(parseDimensions(compact))).toBe(compact);
	});

	test.each(["(1,2,3)", "(0,0,0)"])("coordinates '%s'", (compact) => {
		expect(formatCoordinates(parseCoordinates(compact))).toBe(compact);
	});

	test.each(["10x20x30 (1,2,3)"])("item '%s'", (compact) => {
		expect(format(parseItem(compact))).toBe(compact);
	});
});
