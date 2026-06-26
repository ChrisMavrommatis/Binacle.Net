// mirrors src/ProtocolReader.ts
// ports C#: ProtocolReaderTests. TS reads into JS numbers (no generic T); 64-bit uses float math, so
// only values up to 2^53 are exact — the wide little-endian rows are C#-only (see the provider note).
import {ProtocolReader} from "../src/ProtocolReader";
import {ProtocolWriter} from "../src/ProtocolWriter";
import {Sizes} from "../src/utils";
import {uint16Cases, uint32Cases, uint64Cases} from "./providers/littleEndianCases";

function readerOver(bytes: number[]): ProtocolReader {
	return new ProtocolReader(new DataView(new Uint8Array(bytes).buffer));
}

describe("ProtocolReader", () => {
	describe("reads little-endian", () => {
		// ports C#: ReadUInt16_Reads_Little_Endian
		test.each(uint16Cases)("uint16 — $name", ({value, bytes}) => {
			expect(readerOver(bytes).readUint16()).toBe(value);
		});

		// ports C#: ReadUInt32_Reads_Little_Endian
		test.each(uint32Cases)("uint32 — $name", ({value, bytes}) => {
			expect(readerOver(bytes).readUint32()).toBe(value);
		});

		// ports C#: ReadUInt64_Reads_Little_Endian
		test.each(uint64Cases)("uint64 — $name", ({value, bytes}) => {
			expect(readerOver(bytes).readUint64()).toBe(value);
		});
	});

	// ports C#: Writer_Then_Reader_RoundTrips_Each_Width
	describe("round trips what the writer wrote", () => {
		test.each([
			{name: "8-bit", write: (w: ProtocolWriter) => w.writeByte(0xab), read: (r: ProtocolReader) => r.readByte(), size: 1, value: 0xab},
			{name: "16-bit", write: (w: ProtocolWriter) => w.writeUInt16(0x0102), read: (r: ProtocolReader) => r.readUint16(), size: 2, value: 0x0102},
			{name: "32-bit", write: (w: ProtocolWriter) => w.writeUInt32(0x01020304), read: (r: ProtocolReader) => r.readUint32(), size: 4, value: 0x01020304},
			{name: "64-bit", write: (w: ProtocolWriter) => w.writeUInt64(0x100000000), read: (r: ProtocolReader) => r.readUint64(), size: 8, value: 0x100000000},
		])("$name", ({write, read, size, value}) => {
			const writer = new ProtocolWriter(size);
			write(writer);

			const reader = new ProtocolReader(new DataView(writer.buffer.buffer));

			expect(read(reader)).toBe(value);
		});
	});

	// No C# port: this guards a TS-only limit. A C#-made buffer can carry a 64-bit value above what JS
	// holds exactly; we refuse it instead of returning a silently-rounded number. See vipaq/PROTOCOL.md.
	describe("readUint64 guards the interoperable ceiling", () => {
		test("reads the largest in-range value", () => {
			// maxInteger (2^53 - 1) little-endian: low word all set, high word stops at 0x1F.
			const maxBytes = [0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x1f, 0x00];
			expect(readerOver(maxBytes).readUint64()).toBe(Sizes.maxInteger);
		});

		test("throws when the bytes decode above maxInteger", () => {
			// 2^53, one over the ceiling: little-endian of 0x0020000000000000.
			const overCeiling = [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00];
			expect(() => readerOver(overCeiling).readUint64()).toThrow();
		});
	});
});
