// mirrors src/ProtocolWriter.ts
// ports C#: ProtocolWriterTests
import {ProtocolWriter} from "../src/ProtocolWriter";
import {Sizes} from "../src/utils";
import {expectBytes} from "./support/bytes";
import {uint8Cases, uint16Cases} from "./providers/LittleEndianCases";

// Each write primitive range-checks like C#'s CreateChecked. A value one over its ceiling, or negative, must
// throw rather than truncate; the largest in-range value still writes. maxBytes is `max` little-endian.
const widthCeilings = [
	{name: "8-bit", size: 1, max: Sizes.eightBitsMax, write: (w: ProtocolWriter, v: number) => w.write8Bits(v), maxBytes: [0xff]},
	{name: "16-bit", size: 2, max: Sizes.sixteenBitsMax, write: (w: ProtocolWriter, v: number) => w.write16Bits(v), maxBytes: [0xff, 0xff]},
];

describe("ProtocolWriter", () => {
	describe("writes little-endian", () => {
		// ports C#: Write8Bits_Narrows_T_And_Writes
		test.each(uint8Cases)("8-bit — $name", ({value, bytes}) => {
			const writer = new ProtocolWriter(1);
			writer.write8Bits(value);
			expectBytes(writer.buffer, bytes);
		});

		// ports C#: Write16Bits_Narrows_T_And_Writes_Little_Endian
		test.each(uint16Cases)("16-bit — $name", ({value, bytes}) => {
			const writer = new ProtocolWriter(2);
			writer.write16Bits(value);
			expectBytes(writer.buffer, bytes);
		});
	});

	// ports C#: Write*Bits_Throws_When_Value_Exceeds_* (C# uses CreateChecked; we range-check by hand)
	describe("rejects values that do not fit the width", () => {
		test.each(widthCeilings)("throws when a $name value is one over its ceiling", ({size, max, write}) => {
			expect(() => write(new ProtocolWriter(size), max + 1)).toThrow();
		});

		test.each(widthCeilings)("throws on a negative $name value", ({size, write}) => {
			expect(() => write(new ProtocolWriter(size), -1)).toThrow();
		});
	});

	describe("writes the largest in-range value of each width", () => {
		test.each(widthCeilings)("$name", ({size, max, write, maxBytes}) => {
			const writer = new ProtocolWriter(size);
			write(writer, max);
			expectBytes(writer.buffer, maxBytes);
		});
	});
});
