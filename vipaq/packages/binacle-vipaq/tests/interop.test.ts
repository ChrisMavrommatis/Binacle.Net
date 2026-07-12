// ports C#: InteropDecodeTests
//
// The interop artifacts both serialize the shared input.json; each blob must deserialize back to it. This
// decodes BOTH artifact-cs.json (produced by C#) and artifact-ts.json (produced by TS) through the TS
// deserializer — so TS reads its own output AND C#'s. The two header bytes are pinned first (version, layout and
// all three widths), then the decoded bin/items must equal the input. Every interop blob is uncompressed for now
// (compression is deferred, PROTOCOL.md §6).
import ViPaqSerializer from "../src/ViPaqSerializer";
import {headerFromBytes} from "../src/utils";
import {loadInteropArtifactCases} from "./providers/InteropArtifacts";

describe("interop artifacts deserialize to their input", () => {
	test.each(loadInteropArtifactCases())("$label", async ({bytes, expectedHeader, bin, items}) => {
		// The two header bytes confirm the blob claims the right layout + widths.
		expect(headerFromBytes(bytes[0], bytes[1])).toEqual(expectedHeader);

		const result = await ViPaqSerializer.deserialize(new Uint8Array(bytes));
		expect(result.bin).toEqual(bin);
		expect(result.items).toEqual(items);
	});
});
