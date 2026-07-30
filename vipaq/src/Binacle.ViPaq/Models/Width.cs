namespace Binacle.ViPaq;

// How many bytes one integer takes on the wire (PROTOCOL.md §4).
//
// Codes 2 and 3 are named but carry no width. They exist so the two spare codes in every width field are
// visible in the code, and so a decoder rejects them by name rather than by falling off a switch. An encoder
// must never write one.
internal enum Width
{
	Eight = 0,
	Sixteen = 1,
	Reserved2 = 2,
	Reserved3 = 3
}
