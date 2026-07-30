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
// EncodingInfo used. `compressed` and `layout` are the caller's options through ViPaqSerializationOptions
// (defaults off / row-major, D16); the encoder obeys whatever they are set to.
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
