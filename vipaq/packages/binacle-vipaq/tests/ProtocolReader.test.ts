// mirrors src/ProtocolReader.ts
// ports C#: ProtocolReaderTests. TS reads into JS numbers, with no generic T. Nothing is range-checked: a
// 16-bit value is always in range.
import {ProtocolReader} from "../src/ProtocolReader";
import {ProtocolWriter} from "../src/ProtocolWriter";
import {uint8Cases, uint16Cases} from "./providers/LittleEndianCases";

function readerOver(bytes: number[]): ProtocolReader {
	return new ProtocolReader(new DataView(new Uint8Array(bytes).buffer));
}

describe("ProtocolReader", () => {
	describe("reads little-endian", () => {
		// ports C#: Read8Bits_Reads_The_Byte
		test.each(uint8Cases)("8-bit — $name", ({value, bytes}) => {
			expect(readerOver(bytes).read8Bits()).toBe(value);
		});

		// ports C#: Read16Bits_Widens_To_T
		test.each(uint16Cases)("16-bit — $name", ({value, bytes}) => {
			expect(readerOver(bytes).read16Bits()).toBe(value);
		});
	});

	// ports C#: Writer_Then_Reader_RoundTrips_Each_Width
	describe("round trips what the writer wrote", () => {
		test.each([
			{name: "8-bit", write: (w: ProtocolWriter) => w.write8Bits(0xab), read: (r: ProtocolReader) => r.read8Bits(), size: 1, value: 0xab},
			{name: "16-bit", write: (w: ProtocolWriter) => w.write16Bits(0x0102), read: (r: ProtocolReader) => r.read16Bits(), size: 2, value: 0x0102},
		])("$name", ({write, read, size, value}) => {
			const writer = new ProtocolWriter(size);
			write(writer);

			const reader = new ProtocolReader(new DataView(writer.buffer.buffer));

			expect(read(reader)).toBe(value);
		});
	});
});
