// mirrors src/ProtocolWriter.ts
// ports C#: ProtocolWriterTests
import {ProtocolWriter} from "../src/ProtocolWriter";
import {Sizes} from "../src/utils";
import {expectBytes} from "./support/bytes";
import {uint16Cases, uint32Cases, uint64Cases} from "./providers/littleEndianCases";

// Each write primitive range-checks like C#'s CreateChecked. The 64-bit ceiling is maxInteger
// (2^53 - 1), the protocol's interoperable limit — see vipaq/PROTOCOL.md. A value one over its
// ceiling (or negative) must throw, not truncate; the largest in-range value still writes.
// maxBytes is the little-endian encoding of `max` (uint64's high word stops at 0x1F = 2^53 - 1).
const widthCeilings = [
	{name: "byte", size: 1, max: Sizes.byteMaxSize, write: (w: ProtocolWriter, v: number) => w.writeByte(v), maxBytes: [0xff]},
	{name: "uint16", size: 2, max: Sizes.uShortMaxValue, write: (w: ProtocolWriter, v: number) => w.writeUInt16(v), maxBytes: [0xff, 0xff]},
	{name: "uint32", size: 4, max: Sizes.uIntMaxValue, write: (w: ProtocolWriter, v: number) => w.writeUInt32(v), maxBytes: [0xff, 0xff, 0xff, 0xff]},
	{name: "uint64", size: 8, max: Sizes.maxInteger, write: (w: ProtocolWriter, v: number) => w.writeUInt64(v), maxBytes: [0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x1f, 0x00]},
];

describe("ProtocolWriter", () => {
	describe("writes little-endian", () => {
		// ports C#: WriteUInt16_Writes_Little_Endian
		test.each(uint16Cases)("uint16 — $name", ({value, bytes}) => {
			const writer = new ProtocolWriter(2);
			writer.writeUInt16(value);
			expectBytes(writer.buffer, bytes);
		});

		// ports C#: WriteUInt32_Writes_Little_Endian
		test.each(uint32Cases)("uint32 — $name", ({value, bytes}) => {
			const writer = new ProtocolWriter(4);
			writer.writeUInt32(value);
			expectBytes(writer.buffer, bytes);
		});

		// ports C#: WriteUInt64_Writes_Little_Endian (the wide rows are C#-only, see the provider note)
		test.each(uint64Cases)("uint64 — $name", ({value, bytes}) => {
			const writer = new ProtocolWriter(8);
			writer.writeUInt64(value);
			expectBytes(writer.buffer, bytes);
		});
	});

	// ports C#: WriteByte_Writes_The_Byte
	test("writes a single byte unchanged", () => {
		const writer = new ProtocolWriter(1);
		writer.writeByte(0xab);
		expectBytes(writer.buffer, [0xab]);
	});

	// ports C#: WriteAs*_Throws_When_Value_Does_Not_Fit (C# uses CreateChecked; we range-check by hand)
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
