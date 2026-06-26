// mirrors src/ProtocolWriter.ts
// ports C#: ProtocolWriterTests
import {ProtocolWriter} from "../src/ProtocolWriter";
import {expectBytes} from "./support/bytes";
import {uint16Cases, uint32Cases, uint64Cases} from "./providers/littleEndianCases";

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
});
