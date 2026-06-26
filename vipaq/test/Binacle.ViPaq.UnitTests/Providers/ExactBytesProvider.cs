using System.Collections;
using Binacle.ViPaq.UnitTests.Models;

namespace Binacle.ViPaq.UnitTests.Providers;

// Golden vectors for the whole wire layout: a bin and its items paired with the exact bytes
// SerializeInt32 must produce. Each row was worked out by hand from the format, so a change in
// byte order, header packing, or field order shows up as a mismatch. The TypeScript mirror must
// produce these same bytes for the same input, so this is the cross-language contract.
// Layout: header byte, then little-endian uint16 item count, then bin length/width/height,
// then each item's length/width/height followed by its x/y/z, every value at its section width.
internal class ExactBytesProvider : IEnumerable<object[]>
{
	public IEnumerator<object[]> GetEnumerator()
	{
		// One 8-bit item. Everything fits in a byte, so the header is all zeros.
		yield return
		[
			new Bin<int> { Length = 10, Width = 20, Height = 30 },
			new[] { new Item<int> { Length = 1, Width = 2, Height = 3, X = 4, Y = 5, Z = 6 } },
			new byte[]
			{
				0b00_00_00_00,    // header: Uncompressed, bin/item-dim/item-coord all 8-bit
				0x01, 0x00,       // item count = 1
				0x0A, 0x14, 0x1E, // bin: length 10, width 20, height 30
				0x01, 0x02, 0x03, // item dimensions: length 1, width 2, height 3
				0x04, 0x05, 0x06, // item coordinates: x 4, y 5, z 6
			},
		];

		// Bin needs 16 bits (4096 and 65535 do not fit in a byte). Only the bin-dimensions section
		// widens, so the header's bin bits become 01 and each bin field takes two little-endian bytes.
		yield return
		[
			new Bin<int> { Length = 256, Width = 4096, Height = 65535 },
			new[] { new Item<int> { Length = 1, Width = 2, Height = 3, X = 4, Y = 5, Z = 6 } },
			new byte[]
			{
				0b00_01_00_00,          // header: bin 16-bit, item dims/coords still 8-bit
				0x01, 0x00,             // item count = 1
				0x00, 0x01,             // bin length 256
				0x00, 0x10,             // bin width 4096
				0xFF, 0xFF,             // bin height 65535
				0x01, 0x02, 0x03,       // item dimensions: length 1, width 2, height 3
				0x04, 0x05, 0x06,       // item coordinates: x 4, y 5, z 6
			},
		];

		// Two distinct items. The count is 2 and the two items are written back to back, which
		// pins the per-item stride and the order of items in the body.
		yield return
		[
			new Bin<int> { Length = 10, Width = 20, Height = 30 },
			new[]
			{
				new Item<int> { Length = 1, Width = 2, Height = 3, X = 4, Y = 5, Z = 6 },
				new Item<int> { Length = 7, Width = 8, Height = 9, X = 10, Y = 11, Z = 12 },
			},
			new byte[]
			{
				0b00_00_00_00,    // header: all 8-bit
				0x02, 0x00,       // item count = 2
				0x0A, 0x14, 0x1E, // bin: length 10, width 20, height 30
				0x01, 0x02, 0x03, // item 1 dimensions: length 1, width 2, height 3
				0x04, 0x05, 0x06, // item 1 coordinates: x 4, y 5, z 6
				0x07, 0x08, 0x09, // item 2 dimensions: length 7, width 8, height 9
				0x0A, 0x0B, 0x0C, // item 2 coordinates: x 10, y 11, z 12
			},
		];

		// Item sitting at the origin. Zero coordinates still take their three bytes, so the body
		// length does not change.
		yield return
		[
			new Bin<int> { Length = 10, Width = 20, Height = 30 },
			new[] { new Item<int> { Length = 1, Width = 2, Height = 3, X = 0, Y = 0, Z = 0 } },
			new byte[]
			{
				0b00_00_00_00,    // header: all 8-bit
				0x01, 0x00,       // item count = 1
				0x0A, 0x14, 0x1E, // bin: length 10, width 20, height 30
				0x01, 0x02, 0x03, // item dimensions: length 1, width 2, height 3
				0x00, 0x00, 0x00, // item coordinates: x 0, y 0, z 0
			},
		];
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
