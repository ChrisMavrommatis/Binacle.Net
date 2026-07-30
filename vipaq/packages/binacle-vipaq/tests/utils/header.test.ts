// mirrors src/utils/headerToBytes.ts + headerFromBytes.ts (inverse pair -> one file)
// ports C#: HeaderBytesTests. All 32 combos: every header notation packs to its two bytes and back.
//
// Folded in from the deleted encodingInfo.test.ts (1-byte header) and writeEncodingInfoToBuffer.test.ts. The
// header is two bytes now, packed by headerToBytes; the old "header lands at index 0, body follows" check is
// covered by the exact-bytes vectors (ViPaqSerializer.test.ts), which pin bytes 0-1 as the header.
//
// Also folded away: the compression tests (compressBuffer.test.ts, getDecodingDataStream.test.ts) are gone
// because compression is deferred (PROTOCOL.md §6) — they return when the codec is chosen. The reserved-version
// reject those exercised is now pinned by ViPaqSerializer.test.ts ("on a reserved version") and headerFromBytes.
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
