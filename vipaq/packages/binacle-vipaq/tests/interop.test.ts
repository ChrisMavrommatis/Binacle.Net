// ports C#: InteropDecodeTests
//
// Every interop artifact — from either producer (artifact-cs.*.json, artifact-ts.*.json) and in every codec
// (raw/deflate/gzip) — must deserialize back to its input through the TS library. So TS reads its own output AND
// C#'s, in all three codecs; the C# suite does the mirror. The two header bytes are pinned first (version,
// compression, layout, all widths), then the decoded bin/items must equal the input.
//
// Every artifact decodes the same way — ProtocolEncoder + the codec named by the file (raw = NoOp, which leaves
// the body untouched) — so there is no special case per codec. Compressed bytes are never compared across
// languages (PROTOCOL.md §6.1) — decode-to-input is the whole contract.
import {ProtocolEncoder} from "../src/ProtocolEncoder";
import {CompressionCodec, deflateCodec, gzipCodec, noOpCodec} from "../src/compression";
import {Header} from "../src/models";
import {headerFromBytes} from "../src/utils";
import {ArtifactCodec, loadInteropArtifactCases} from "./providers/InteropArtifacts";

const codecFor: Record<ArtifactCodec, CompressionCodec> = {
	raw: noOpCodec,
	deflate: deflateCodec,
	gzip: gzipCodec,
};

describe("interop artifacts deserialize to their input, in every codec, from both producers", () => {
	test.each(loadInteropArtifactCases())("$label", async ({codec, bytes, expectedHeader, bin, items}) => {
		const blob = new Uint8Array(bytes);

		// The two header bytes confirm the blob claims the right compression flag, layout and widths.
		expect(headerFromBytes(blob[0], blob[1])).toEqual(expectedHeader);

		const result = await new ProtocolEncoder(codecFor[codec]).decode(expectedHeader, blob.slice(Header.byteCount));

		expect(result.bin).toEqual(bin);
		expect(result.items).toEqual(items);
	});
});
