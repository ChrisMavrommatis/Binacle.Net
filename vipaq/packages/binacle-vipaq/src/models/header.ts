import {Layout} from "./layout";
import {Version} from "./version";
import {Width} from "./width";

// Ports C#: Header (the record struct's data). The two header bytes (PROTOCOL.md §2), also the encoder's
// directive:
//
//   Byte 0 — form                                Byte 1 — widths
//   [Version][Compressed][Layout][reserved]      [Bin dims][Item dims][Item coords][reserved]
//   [2 bits ][1 bit     ][1 bit ][4 bits  ]      [2 bits  ][2 bits   ][2 bits     ][2 bits  ]
//
// Data only. Behaviour lives in the utils that build it (createHeader), pack it (headerToBytes), read it back
// (headerFromBytes) and name it (headerNotation). `compressed` and `layout` come from
// ViPaqSerializationOptions; the encoder obeys whatever they are set to.
export default class Header {
	// Always two bytes, never compressed.
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
