// mirrors src/utils/headerToBytes.ts + headerFromBytes.ts (inverse pair -> one file)
// ports C#: HeaderBytesTests. All 32 combos: every header notation packs to its two bytes and back.
//
// "Header lands at index 0, body follows" is covered by the exact-bytes vectors in ViPaqSerializer.test.ts,
// which pin bytes 0-1 as the header.
import {headerBytesCases} from "../providers/HeaderBytesCases";
import {headerToBytes, headerFromBytes} from "../../src/utils";

describe("header packing", () => {
	// ports C#: ToBytes_Returns_Correct_Bytes
	test.each(headerBytesCases)("$notation packs to its two bytes", ({header, bytes}) => {
		expect(Array.from(headerToBytes(header))).toEqual(bytes);
	});

	// ports C#: FromBytes_Returns_Correct_Header
	test.each(headerBytesCases)("$notation unpacks from its two bytes", ({header, bytes}) => {
		expect(headerFromBytes(bytes[0], bytes[1])).toEqual(header);
	});

	// ports C#: ToBytes_Then_FromBytes_Returns_Original
	test.each(headerBytesCases)("$notation round trips", ({header}) => {
		const bytes = headerToBytes(header);
		expect(headerFromBytes(bytes[0], bytes[1])).toEqual(header);
	});
});
