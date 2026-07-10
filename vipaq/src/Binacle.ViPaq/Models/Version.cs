namespace Binacle.ViPaq;

// The wire format version — bits 7-6 of header byte 0 (PROTOCOL.md §2.1).
//
// A change the flags and the width codes cannot express — a different compression codec, a new body shape —
// takes the next code. The codec is pinned by this field, which is why there is no codec field on the wire (§6).
internal enum Version
{
	Version1 = 0,
	Reserved1 = 1,
	Reserved2 = 2,
	Reserved3 = 3
}
