import * as fs from "fs";
import * as path from "path";
import {parseDimensions as parseBin, parseItems} from "binacle-compact-notation";
import {ProtocolEncoder} from "../src/ProtocolEncoder";
import {parseHeader} from "../src/headerNotation";
import {Artifact} from "./Artifact";

// Ports C#: InteropArtifactGenerator. Encodes each shared interop input with the TS ViPaq library and writes the
// bytes (base64) to artifact-ts.json. Mirrors the C# generator off the same input.
//
// It drives ProtocolEncoder, not ViPaqSerializer, so it obeys each scenario's ExpectedHeader — that is what lets
// it emit the columnar and wider scenarios ViPaqSerializer's narrowest-raw choice would not. Every scenario is
// uncompressed for now (compression is deferred, PROTOCOL.md §6), so an uncompressed blob is byte-identical to
// the C# producer's, which the byte-identity test checks.

interface InputScenario {
	Name: string;
	ExpectedHeader: string;
	Bin: string;
	Items: string[];
}

export async function generateInteropArtifact(): Promise<void> {
	const interopDir = path.join(__dirname, "..", "..", "..", "test-vectors", "interop");
	const inputPath = path.join(interopDir, "input.json");
	const outputPath = path.join(interopDir, "artifact-ts.json");

	const inputs: InputScenario[] = JSON.parse(fs.readFileSync(inputPath, "utf8"));

	const encoder = new ProtocolEncoder();
	const artifacts: Artifact[] = [];
	for (const input of inputs) {
		const bin = parseBin(input.Bin);
		const items = parseItems(input.Items);
		const header = parseHeader(input.ExpectedHeader);

		const bytes = await encoder.encode(header, bin, items);

		artifacts.push(new Artifact(
			input.Name,
			"typescript",
			Buffer.from(bytes).toString("base64"),
		));
	}

	// Tabs, expanded — matches the C# interop generator's WriteIndented output style.
	fs.writeFileSync(outputPath, JSON.stringify(artifacts, null, "\t"));

	console.log(`Wrote ${artifacts.length} artifact(s) to ${outputPath}`);
	for (const artifact of artifacts) {
		console.log(`  ${artifact.Name} (${artifact.Base64.length} base64 chars)`);
	}
}
