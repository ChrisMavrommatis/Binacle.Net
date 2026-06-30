// mirrors src/ProtocolWriter.ts
// ports C#: ProtocolWriterTests
import {ProtocolWriter} from "../src/ProtocolWriter";
import {Sizes} from "../src/utils";
import {expectBytes} from "./support/bytes";
import {uint16Cases, uint32Cases, uint64Cases} from "./providers/littleEndianCases";

// Each write primitive range-checks like C#'s CreateChecked. The 64-bit ceiling is maxInteger
// (2^53 - 1), the protocol's interoperable limit — see vipaq/PROTOCOL.md. A value one over its
// ceiling (or negative) must throw, not truncate; the largest in-range value still writes.
// maxBytes is the little-endian encoding of `max` (64-bit's high word stops at 0x1F = 2^53 - 1).
const widthCeilings = [
	{name: "8-bit", size: 1, max: Sizes.eightBitsMax, write: (w: ProtocolWriter, v: number) => w.write8Bits(v), maxBytes: [0xff]},
	{name: "16-bit", size: 2, max: Sizes.sixteenBitsMax, write: (w: ProtocolWriter, v: number) => w.write16Bits(v), maxBytes: [0xff, 0xff]},
	{name: "32-bit", size: 4, max: Sizes.thirtyTwoBitsMax, write: (w: ProtocolWriter, v: number) => w.write32Bits(v), maxBytes: [0xff, 0xff, 0xff, 0xff]},
	{name: "64-bit", size: 8, max: Sizes.maxInteger, write: (w: ProtocolWriter, v: number) => w.write64Bits(v), maxBytes: [0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x1f, 0x00]},
];

describe("ProtocolWriter", () => {
	describe("writes little-endian", () => {
		// ports C#: Write16Bits_Narrows_T_And_Writes_Little_Endian
		test.each(uint16Cases)("16-bit — $name", ({value, bytes}) => {
			const writer = new ProtocolWriter(2);
			writer.write16Bits(value);
			expectBytes(writer.buffer, bytes);
		});

		// ports C#: Write32Bits_Narrows_T_And_Writes_Little_Endian
		test.each(uint32Cases)("32-bit — $name", ({value, bytes}) => {
			const writer = new ProtocolWriter(4);
			writer.write32Bits(value);
			expectBytes(writer.buffer, bytes);
		});

		// ports C#: Write64Bits_Narrows_T_And_Writes_Little_Endian (the wide rows are C#-only, see the provider note)
		test.each(uint64Cases)("64-bit — $name", ({value, bytes}) => {
			const writer = new ProtocolWriter(8);
			writer.write64Bits(value);
			expectBytes(writer.buffer, bytes);
		});
	});

	// ports C#: Write8Bits_Narrows_T_And_Writes
	test("writes a single byte unchanged", () => {
		const writer = new ProtocolWriter(1);
		writer.write8Bits(0xab);
		expectBytes(writer.buffer, [0xab]);
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
