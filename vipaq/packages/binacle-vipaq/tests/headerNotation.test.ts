// mirrors src/headerNotation.ts (no direct C# test — the C# HeaderNotation is exercised indirectly; TS pins it
// directly here since the vectors depend on it). Every one of the 32 header combos must round-trip through
// parse/format, and a malformed label must be rejected. Reuses header-bytes.json (notation + parsed header).
import {parseHeader, formatHeader} from "../src/headerNotation";
import {headerBytesCases} from "./providers/HeaderBytesCases";

describe("header notation", () => {
	test.each(headerBytesCases)("$notation round-trips through parse/format", ({notation, header}) => {
		expect(formatHeader(header)).toBe(notation);
		expect(parseHeader(notation)).toEqual(header);
	});

	describe("rejects a malformed notation", () => {
		test.each([
			{name: "too few tokens", notation: "v1_raw_row_8_8"},
			{name: "unknown version", notation: "v2_raw_row_8_8_8"},
			{name: "unknown compression", notation: "v1_zip_row_8_8_8"},
			{name: "unknown layout", notation: "v1_raw_diag_8_8_8"},
			{name: "reserved width", notation: "v1_raw_row_8_8_32"},
		])("$name", ({notation}) => {
			expect(() => parseHeader(notation)).toThrow();
		});
	});
});
