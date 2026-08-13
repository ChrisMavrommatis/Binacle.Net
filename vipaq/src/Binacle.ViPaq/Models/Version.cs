namespace Binacle.ViPaq;

// The wire format version, bits 7-6 of header byte 0 (PROTOCOL.md §2.1). A change the flags and width codes
// cannot express takes the next code. This field pins the codec, which is why the wire has no codec field (§6).
internal enum Version
{
	Version1 = 0,
	Reserved1 = 1,
	Reserved2 = 2,
	Reserved3 = 3
}
