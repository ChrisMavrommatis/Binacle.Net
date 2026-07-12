import {Header, Layout, Version, Width} from "./models";

// Ports C#: HeaderNotation. Text form of a Header, used by the test vectors and the interop generator so a
// vector can name the exact header its bytes were produced under. Wire-specific (it names Header/Width/Layout/
// Version), so it stays in the vipaq mirror; the geometry notation lives in binacle-compact-notation.
//
// Grammar (six underscore-delimited tokens, in wire order):
//
//   v{N}_{raw|comp}_{row|col}_{binW}_{itemDimW}_{itemCoordW}
//
//   v1_raw_row_8_8_8      uncompressed, row-major, all 8-bit
//   v1_raw_col_16_16_16   uncompressed, columnar, all 16-bit
//
// Version leads and is mandatory (positional parsing stays forward-safe). The compressed flag is a bare bit
// (`comp`, never a codec name) — the header carries no codec. Widths are only `8` or `16`; reserved codes never
// reach the wire, so they never reach the notation. A bad label is not a bad blob, so a bad token throws a plain
// Error (mirrors C# throwing FormatException, not ViPaqFormatException).

const separator = "_";
const tokenCount = 6;

export function parseHeader(notation: string): Header {
	const tokens = notation.split(separator);
	if (tokens.length !== tokenCount) {
		throw new Error(
			`Header notation must have ${tokenCount} tokens separated by '${separator}', got ${tokens.length}: '${notation}'`,
		);
	}
	return new Header(
		parseVersion(tokens[0]),
		parseCompressed(tokens[1]),
		parseLayout(tokens[2]),
		parseWidth(tokens[3], "bin dimensions"),
		parseWidth(tokens[4], "item dimensions"),
		parseWidth(tokens[5], "item coordinates"),
	);
}

export function formatHeader(header: Header): string {
	return [
		formatVersion(header.version),
		header.compressed ? "comp" : "raw",
		formatLayout(header.layout),
		formatWidth(header.binDimensionsWidth),
		formatWidth(header.itemDimensionsWidth),
		formatWidth(header.itemCoordinatesWidth),
	].join(separator);
}

function parseVersion(token: string): Version {
	if (token === "v1") return Version.Version1;
	throw new Error(`Unknown version token '${token}', this implementation reads 'v1'`);
}

function formatVersion(version: Version): string {
	if (version === Version.Version1) return "v1";
	throw new Error(`Only version ${Version.Version1} has a notation`);
}

function parseCompressed(token: string): boolean {
	if (token === "comp") return true;
	if (token === "raw") return false;
	throw new Error(`Unknown compression token '${token}', expected 'raw' or 'comp'`);
}

function parseLayout(token: string): Layout {
	if (token === "row") return Layout.RowMajor;
	if (token === "col") return Layout.Columnar;
	throw new Error(`Unknown layout token '${token}', expected 'row' or 'col'`);
}

function formatLayout(layout: Layout): string {
	if (layout === Layout.RowMajor) return "row";
	if (layout === Layout.Columnar) return "col";
	throw new Error(`Unknown layout ${layout}`);
}

function parseWidth(token: string, section: string): Width {
	if (token === "8") return Width.Eight;
	if (token === "16") return Width.Sixteen;
	throw new Error(`Unknown width token '${token}' for ${section}, expected '8' or '16'`);
}

function formatWidth(width: Width): string {
	if (width === Width.Eight) return "8";
	if (width === Width.Sixteen) return "16";
	throw new Error(`Reserved width ${width} has no notation`);
}
