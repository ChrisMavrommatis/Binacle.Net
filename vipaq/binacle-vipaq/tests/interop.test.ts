// ports C#: InteropDecodeTests
//
// The interop artifacts both serialize the shared input.json; each blob must deserialize back to it. This
// decodes BOTH artifact-cs.json (produced by C#) and artifact-ts.json (produced by TS) through the TS
// deserializer — so TS reads its own output AND C#'s. byte 0 is pinned first (Version + all three widths),
// then the decoded bin/items must equal the input. Compressed blobs are never byte-compared, only decoded.
import ViPaqSerializer from "../src/ViPaqSerializer";
import {encodingInfoFromByte} from "../src/utils";
import {interopArtifactCases} from "./providers/InteropArtifacts";

describe("interop artifacts deserialize to their input", () => {
	test.each(interopArtifactCases)("$label", async ({bytes, expected, bin, items}) => {
		// byte 0 confirms the blob claims the right compression flag + widths.
		expect(encodingInfoFromByte(bytes[0])).toEqual(expected);

		const result = await ViPaqSerializer.deserialize(new Uint8Array(bytes));
		expect(result.bin).toEqual(bin);
		expect(result.items).toEqual(items);
	});
});
