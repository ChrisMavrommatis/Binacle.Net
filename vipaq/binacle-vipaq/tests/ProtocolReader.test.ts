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
		// ports C#: Read16Bits_Widens_To_T
		test.each(uint16Cases)("16-bit — $name", ({value, bytes}) => {
			expect(readerOver(bytes).read16Bits()).toBe(value);
		});

		// ports C#: Read32Bits_Widens_To_T
		test.each(uint32Cases)("32-bit — $name", ({value, bytes}) => {
			expect(readerOver(bytes).read32Bits()).toBe(value);
		});

		// ports C#: Read64Bits_Widens_To_T
		test.each(uint64Cases)("64-bit — $name", ({value, bytes}) => {
			expect(readerOver(bytes).read64Bits()).toBe(value);
		});
	});

	// ports C#: Writer_Then_Reader_RoundTrips_Each_Width
	describe("round trips what the writer wrote", () => {
		test.each([
			{name: "8-bit", write: (w: ProtocolWriter) => w.write8Bits(0xab), read: (r: ProtocolReader) => r.read8Bits(), size: 1, value: 0xab},
			{name: "16-bit", write: (w: ProtocolWriter) => w.write16Bits(0x0102), read: (r: ProtocolReader) => r.read16Bits(), size: 2, value: 0x0102},
			{name: "32-bit", write: (w: ProtocolWriter) => w.write32Bits(0x01020304), read: (r: ProtocolReader) => r.read32Bits(), size: 4, value: 0x01020304},
			{name: "64-bit", write: (w: ProtocolWriter) => w.write64Bits(0x100000000), read: (r: ProtocolReader) => r.read64Bits(), size: 8, value: 0x100000000},
		])("$name", ({write, read, size, value}) => {
			const writer = new ProtocolWriter(size);
			write(writer);

			const reader = new ProtocolReader(new DataView(writer.buffer.buffer));

			expect(read(reader)).toBe(value);
		});
	});

	// No C# port: this guards a TS-only limit. A C#-made buffer can carry a 64-bit value above what JS
	// holds exactly; we refuse it instead of returning a silently-rounded number. See vipaq/PROTOCOL.md.
	describe("read64Bits guards the interoperable ceiling", () => {
		test("reads the largest in-range value", () => {
			// maxInteger (2^53 - 1) little-endian: low word all set, high word stops at 0x1F.
			const maxBytes = [0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x1f, 0x00];
			expect(readerOver(maxBytes).read64Bits()).toBe(Sizes.maxInteger);
		});

		test("throws when the bytes decode above maxInteger", () => {
			// 2^53, one over the ceiling: little-endian of 0x0020000000000000.
			const overCeiling = [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00];
			expect(() => readerOver(overCeiling).read64Bits()).toThrow();
		});
	});
});
