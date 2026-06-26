// mirrors src/utils/getByteSize.ts
// TS-specific: C# streams its output and never pre-sizes a buffer, so it has no getByteSize. This maps
// a BitSize to its wire byte width; getBufferSize multiplies by it. Bug #1 lived here (32/64 gave 3/4).
import {getByteSize} from "../../src/utils";
import {BitSize} from "../../src/models";

describe("getByteSize", () => {
	test.each([
		{name: "Eight", bitSize: BitSize.Eight, bytes: 1},
		{name: "Sixteen", bitSize: BitSize.Sixteen, bytes: 2},
		{name: "ThirtyTwo", bitSize: BitSize.ThirtyTwo, bytes: 4},
		{name: "SixtyFour", bitSize: BitSize.SixtyFour, bytes: 8},
	])("maps $name to its byte width", ({bitSize, bytes}) => {
		expect(getByteSize(bitSize)).toBe(bytes);
	});

	test("throws on an unknown bit size", () => {
		expect(() => getByteSize(99 as BitSize)).toThrow();
	});
});
