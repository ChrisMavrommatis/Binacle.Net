// mirrors src/utils/writeEncodingInfoToBuffer.ts
// TS-specific: C# writes the header to the stream first; TS builds the body, then prepends the header
// byte by allocating len+1 and copying. This pins that the header lands at index 0 and the body follows.
import {writeEncodingInfoToBuffer, encodingInfoToByte} from "../../src/utils";
import {BitSize, EncodingInfo, Version} from "../../src/models";

describe("writeEncodingInfoToBuffer", () => {
	test("prepends the encoding byte at index 0 and keeps the body", () => {
		const info = new EncodingInfo(Version.CompressedGzip, BitSize.Sixteen, BitSize.Eight, BitSize.Eight);
		const body = new Uint8Array([0xaa, 0xbb, 0xcc]);

		const result = writeEncodingInfoToBuffer(info, body);

		expect(result.length).toBe(body.length + 1);
		expect(result[0]).toBe(encodingInfoToByte(info));
		expect(Array.from(result.slice(1))).toEqual([0xaa, 0xbb, 0xcc]);
	});
});
