// mirrors src/utils/widthByteCount.ts
// ports C#: WidthHelper.ByteCount coverage. Maps a Width to its wire byte width; getBodyLength multiplies by it.
// Only Eight and Sixteen have a byte count — a reserved width never reaches the wire, so it throws.
import {widthByteCount} from "../../src/utils";
import {Width} from "../../src/models";

describe("widthByteCount", () => {
	test.each([
		{name: "Eight", width: Width.Eight, bytes: 1},
		{name: "Sixteen", width: Width.Sixteen, bytes: 2},
	])("maps $name to its byte width", ({width, bytes}) => {
		expect(widthByteCount(width)).toBe(bytes);
	});

	test.each([
		{name: "Reserved2", width: Width.Reserved2},
		{name: "Reserved3", width: Width.Reserved3},
	])("throws on the reserved width $name", ({width}) => {
		expect(() => widthByteCount(width)).toThrow();
	});
});
