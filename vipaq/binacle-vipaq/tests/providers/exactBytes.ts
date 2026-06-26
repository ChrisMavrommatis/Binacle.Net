// Golden vectors for the whole wire layout: a named (bin, items) input paired with the exact bytes
// ViPaqSerializer.serialize must produce. Mirrors C# Providers/ExactBytesProvider.cs — same inputs,
// same bytes. This is the cross-language contract; Session 2 lifts these rows into the shared
// inputs.json (keyed by these names).
//
// Layout per blob: header byte, then little-endian uint16 item count, then bin length/width/height,
// then each item's length/width/height followed by its x/y/z, every value at its section width.
// Not a *.test.ts file, so jest does not run it.

import {Coordinates, Dimensions} from "../../src/models";

type Item = Dimensions & Coordinates;

export const exactBytesCases: {name: string; bin: Dimensions; items: Item[]; bytes: number[]}[] = [
	{
		// Everything fits in a byte, so the header is all zeros.
		name: "one 8-bit item",
		bin: {length: 10, width: 20, height: 30},
		items: [{length: 1, width: 2, height: 3, x: 4, y: 5, z: 6}],
		bytes: [
			0b00_00_00_00, // header: uncompressed, bin/item-dim/item-coord all 8-bit
			0x01, 0x00, // item count = 1
			0x0a, 0x14, 0x1e, // bin: length 10, width 20, height 30
			0x01, 0x02, 0x03, // item dimensions: length 1, width 2, height 3
			0x04, 0x05, 0x06, // item coordinates: x 4, y 5, z 6
		],
	},
	{
		// Bin needs 16 bits (4096 and 65535 do not fit in a byte). Only the bin section widens.
		name: "16-bit bin, 8-bit items",
		bin: {length: 256, width: 4096, height: 65535},
		items: [{length: 1, width: 2, height: 3, x: 4, y: 5, z: 6}],
		bytes: [
			0b00_01_00_00, // header: bin 16-bit, item dims/coords still 8-bit
			0x01, 0x00, // item count = 1
			0x00, 0x01, // bin length 256
			0x00, 0x10, // bin width 4096
			0xff, 0xff, // bin height 65535
			0x01, 0x02, 0x03, // item dimensions
			0x04, 0x05, 0x06, // item coordinates
		],
	},
	{
		// Item at the origin. Zero coordinates still take their three bytes.
		name: "item at the origin (zero coords)",
		bin: {length: 10, width: 20, height: 30},
		items: [{length: 1, width: 2, height: 3, x: 0, y: 0, z: 0}],
		bytes: [
			0b00_00_00_00,
			0x01, 0x00,
			0x0a, 0x14, 0x1e,
			0x01, 0x02, 0x03,
			0x00, 0x00, 0x00, // x 0, y 0, z 0
		],
	},
];
