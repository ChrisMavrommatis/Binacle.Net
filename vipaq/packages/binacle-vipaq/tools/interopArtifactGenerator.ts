import * as fs from "fs";
import * as path from "path";
import {parseDimensions as parseBin, parseItems} from "binacle-compact-notation";
import {ProtocolEncoder} from "../src/ProtocolEncoder";
import {CompressionCodec, deflateCodec, gzipCodec, noOpCodec} from "../src/compression";
import {parseHeader} from "../src/headerNotation";
import {Header} from "../src/models";
import {Artifact} from "./Artifact";

// Ports C#: InteropArtifactGenerator. Encodes each shared interop input with the TS ViPaq library and writes the
// bytes (base64) to the interop/ts folder, one file per codec: ts/raw.json, ts/deflate.json, ts/gzip.json.
// Mirrors the C# generator (which writes interop/cs) off the same input.
//
// It drives ProtocolEncoder, not ViPaqSerializer, so it obeys each scenario's ExpectedHeader — that is what lets
// it force compression on and emit the columnar/wider scenarios. Raw uses the NoOp codec; deflate/gzip set the
// header's compressed bit and run the real codec. The cross-language decode test proves each language reads all
// three, from either producer.

interface InputScenario {
	Name: string;
	ExpectedHeader: string;
	Bin: string;
	Items: string[];
}

interface Mode {
	suffix: string;
	compressed: boolean;
	codec: CompressionCodec;
}

const modes: Mode[] = [
	{suffix: "raw", compressed: false, codec: noOpCodec},
	{suffix: "deflate", compressed: true, codec: deflateCodec},
	{suffix: "gzip", compressed: true, codec: gzipCodec},
];

export async function generateInteropArtifact(): Promise<void> {
	const interopDir = path.join(__dirname, "..", "..", "..", "test-vectors", "interop");
	const inputs: InputScenario[] = JSON.parse(fs.readFileSync(path.join(interopDir, "input.json"), "utf8"));

	// This is the TS producer, so it only ever writes its own folder — no language stem anywhere.
	const outputDir = path.join(interopDir, "ts");
	fs.mkdirSync(outputDir, {recursive: true});

	for (const mode of modes) {
		const encoder = new ProtocolEncoder(mode.codec);
		const artifacts: Artifact[] = [];
		for (const input of inputs) {
			const bin = parseBin(input.Bin);
			const items = parseItems(input.Items);

			// The header comes from the scenario; a compressed mode reuses it with the compressed bit set.
			const parsed = parseHeader(input.ExpectedHeader);
			const header = mode.compressed
				? new Header(parsed.version, true, parsed.layout, parsed.binDimensionsWidth, parsed.itemDimensionsWidth, parsed.itemCoordinatesWidth)
				: parsed;

			const bytes = await encoder.encode(header, bin, items);
			artifacts.push(new Artifact(input.Name, "typescript", Buffer.from(bytes).toString("base64")));
		}

		// Tabs, expanded — matches the C# interop generator's WriteIndented output style.
		const outputPath = path.join(outputDir, `${mode.suffix}.json`);
		fs.writeFileSync(outputPath, JSON.stringify(artifacts, null, "\t"));
		console.log(`Wrote ${artifacts.length} ${mode.suffix} artifact(s) to ${outputPath}`);
	}
}
