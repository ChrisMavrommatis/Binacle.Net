import {Layout} from "./layout";
import {Version} from "./version";
import {Width} from "./width";

// Ports C#: Header (the record struct's data). The two header bytes (PROTOCOL.md §2), both what the encoder is
// told to do and what lands on the wire:
//
//   Byte 0 — form                                Byte 1 — widths
//   [Version][Compressed][Layout][reserved]      [Bin dims][Item dims][Item coords][reserved]
//   [2 bits ][1 bit     ][1 bit ][4 bits  ]      [2 bits  ][2 bits   ][2 bits     ][2 bits  ]
//
// Data only. Behaviour lives in the utils that build it (createHeader), pack it (headerToBytes), read it back
// (headerFromBytes) and name it (headerNotation) — the same data-class-plus-free-functions split the old
// EncodingInfo used. `compressed` is a field, but no unit test sets it yet: compression is deferred
// (PROTOCOL.md §6), so ViPaqSerializer always writes it false and refuses to read a blob that has it set.
export default class Header {
	// The header is always two bytes, and it is never compressed.
	public static readonly byteCount = 2;

	constructor(
		public version: Version,
		public compressed: boolean,
		public layout: Layout,
		public binDimensionsWidth: Width,
		public itemDimensionsWidth: Width,
		public itemCoordinatesWidth: Width,
	) {}
}
