import * as fs from "fs";
import * as path from "path";
import ViPaqSerializer from "../src/ViPaqSerializer";
import {encodingInfoFromByte} from "../src/utils";
import {parseBin, parseItems} from "./compactParser";
import {toLabel} from "./encodingInfoLabel";
import {Artifact} from "./Artifact";

// Ports C#: InteropArtifactGenerator. Serializes each shared interop input with the TS ViPaq library and
// writes the bytes (base64) to artifact-ts.json. Mirrors the C# generator off the same input; only the
// compressed bytes differ, because Node's CompressionStream and C#'s GZipStream emit different valid gzip.

interface InputScenario {
	Name: string;
	Bin: string;
	Items: string[];
}

export async function generateInteropArtifact(): Promise<void> {
	const interopDir = path.join(__dirname, "..", "..", "test-vectors", "interop");
	const inputPath = path.join(interopDir, "input.json");
	const outputPath = path.join(interopDir, "artifact-ts.json");

	const inputs: InputScenario[] = JSON.parse(fs.readFileSync(inputPath, "utf8"));

	const artifacts: Artifact[] = [];
	for (const input of inputs) {
		const bin = parseBin(input.Bin);
		const items = parseItems(input.Items);

		const bytes = await ViPaqSerializer.serialize(bin, items);

		artifacts.push(new Artifact(
			input.Name,
			"typescript",
			toLabel(encodingInfoFromByte(bytes[0])),
			Buffer.from(bytes).toString("base64"),
		));
	}

	// Tabs + no trailing newline to match the C# generator's output style.
	fs.writeFileSync(outputPath, JSON.stringify(artifacts, null, "\t"));

	console.log(`Wrote ${artifacts.length} artifact(s) to ${outputPath}`);
	for (const artifact of artifacts) {
		console.log(`  ${artifact.Name} -> ${artifact.EncodingInfo} (${artifact.Base64.length} base64 chars)`);
	}
}
