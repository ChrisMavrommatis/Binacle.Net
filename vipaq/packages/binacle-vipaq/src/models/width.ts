// Ports C#: Width. How wide a section's values are on the wire (PROTOCOL.md §4). Only Eight and Sixteen ever
// reach the wire; codes 2 and 3 are reserved, so a decoder that reads one rejects the blob. The old BitSize had
// ThirtyTwo and SixtyFour too — they are gone: every interoperable value fits in 16 bits (0..65535).
export const enum Width {
	Eight = 0,
	Sixteen = 1,
	Reserved2 = 2,
	Reserved3 = 3,
}
