// Shared little-endian vectors: a named value paired with the exact bytes it occupies on the wire,
// low byte first. Reader and writer tests run the same rows. Mirrors C# Providers/LittleEndianCases.cs.
//
// Each width has three cases: zero, a value whose bytes are all different (pins the order), and all
// bits set. Not a *.test.ts file, so jest does not run it.

type Case = {name: string; value: number; bytes: number[]};

export const uint16Cases: Case[] = [
	{name: "zero", value: 0x0000, bytes: [0x00, 0x00]},
	{name: "ordered bytes", value: 0x0102, bytes: [0x02, 0x01]},
	{name: "all bits set", value: 0xffff, bytes: [0xff, 0xff]},
];

export const uint32Cases: Case[] = [
	{name: "zero", value: 0x00000000, bytes: [0x00, 0x00, 0x00, 0x00]},
	{name: "ordered bytes", value: 0x01020304, bytes: [0x04, 0x03, 0x02, 0x01]},
	{name: "all bits set", value: 0xffffffff, bytes: [0xff, 0xff, 0xff, 0xff]},
];

// JS numbers are exact only to 2^53, so we cannot reuse C#'s 0x0102030405060708 vector here. We use a
// value with a distinct high word that stays under MAX_SAFE_INTEGER. This is a real C#/TS divergence:
// the 64-bit ordered-bytes and all-bits-set rows are C#-only.
export const uint64Cases: Case[] = [
	{name: "zero", value: 0, bytes: [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00]},
	{name: "high word set", value: 0x100000000, bytes: [0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00]},
];
