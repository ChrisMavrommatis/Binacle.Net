// Byte-exact assertions for the wire format.
//
// The format is byte-exact, so most failures are "wrong byte at index N". We compare as hex so the
// failure line reads "00 01 00 0a ..." vs "00 01 00 0b ..." and you can see which byte moved, instead
// of jest's dense numeric array diff. Every byte test fails the same readable way.
//
// Not a *.test.ts file, so jest does not run it — it is imported by the tests that do.

export function toHex(bytes: Uint8Array | number[]): string {
	return Array.from(bytes, b => b.toString(16).padStart(2, "0")).join(" ");
}

export function expectBytes(actual: Uint8Array | number[], expected: number[]): void {
	expect(toHex(actual)).toBe(toHex(expected));
}
