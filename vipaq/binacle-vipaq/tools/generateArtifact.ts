import * as fs from "fs";
import * as path from "path";
import ViPaqSerializer from "../src/ViPaqSerializer";
import {encodingInfoFromByte} from "../src/utils";
import {parseBin, parseItems} from "./compactParser";
import {toLabel} from "./encodingInfoLabel";

// Reads the shared interop inputs, serializes each with the TS ViPaq library, and writes the bytes
// (base64) to artifact-ts.json. Run by hand (npm run generate:interop) when the input scenarios change
// — the output is committed and read by BOTH test suites (see vipaq/test-vectors/interop/). Mirrors the
// C# generator, off the same input; only the compressed bytes differ, because Node's CompressionStream
// and C#'s GZipStream emit different valid gzip.

const interopDir = path.join(__dirname, "..", "..", "test-vectors", "interop");
const inputPath = path.join(interopDir, "input.json");
const outputPath = path.join(interopDir, "artifact-ts.json");

interface InputScenario {
	Name: string;
	Bin: string;
	Items: string[];
}

interface Artifact {
	Name: string;
	Producer: string;
	EncodingInfo: string;
	Base64: string;
}

async function main(): Promise<void> {
	const inputs: InputScenario[] = JSON.parse(fs.readFileSync(inputPath, "utf8"));

	const artifacts: Artifact[] = [];
	for (const input of inputs) {
		const bin = parseBin(input.Bin);
		const items = parseItems(input.Items);

		const bytes = await ViPaqSerializer.serialize(bin, items);

		artifacts.push({
			Name: input.Name,
			Producer: "typescript",
			EncodingInfo: toLabel(encodingInfoFromByte(bytes[0])),
			Base64: Buffer.from(bytes).toString("base64"),
		});
	}

	// Tabs + no trailing newline to match the C# generator's output style.
	fs.writeFileSync(outputPath, JSON.stringify(artifacts, null, "\t"));

	console.log(`Wrote ${artifacts.length} artifact(s) to ${outputPath}`);
	for (const artifact of artifacts) {
		console.log(`  ${artifact.Name} -> ${artifact.EncodingInfo} (${artifact.Base64.length} base64 chars)`);
	}
}

main().catch((error) => {
	console.error(error);
	process.exit(1);
});
