// Byte-exact assertions for the wire format. Compared as hex so a failure reads "00 01 00 0a ..." vs
// "00 01 00 0b ..." and shows which byte moved, instead of jest's dense numeric array diff.
//
// Not a *.test.ts file, so jest does not run it.

export function toHex(bytes: Uint8Array | number[]): string {
	return Array.from(bytes, b => b.toString(16).padStart(2, "0")).join(" ");
}

export function expectBytes(actual: Uint8Array | number[], expected: number[]): void {
	expect(toHex(actual)).toBe(toHex(expected));
}
