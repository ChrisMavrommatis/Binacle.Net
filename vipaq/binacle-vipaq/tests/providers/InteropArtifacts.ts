// Ports C#: Providers/Interop/InteropProvider.cs. The interop artifacts (interop/artifact-cs.json,
// interop/artifact-ts.json) both serialize the shared input.json; each blob must deserialize back to it.
// Reads BOTH artifact files so TS decodes its own output AND the C# output — the cross-language guarantee.
// Each row joins to input.json by Name. Not a *.test.ts file, so jest does not run it.

import {readVectors} from "../support/vectorReader";
import {parseBin, parseItems, parseEncodingInfo} from "../support/vectorParser";
import {Coordinates, Dimensions, EncodingInfo} from "../../src/models";

type Item = Dimensions & Coordinates;

interface InputVector {
	Name: string;
	Bin: string;
	Items: string[];
}

interface ArtifactVector {
	Name: string;
	Producer: string;
	EncodingInfo: string;
	Base64: string;
}

export interface InteropArtifactCase {
	label: string;
	bytes: number[];
	expected: EncodingInfo;
	bin: Dimensions;
	items: Item[];
}

function loadInputs(): Map<string, {bin: Dimensions; items: Item[]}> {
	const inputs = new Map<string, {bin: Dimensions; items: Item[]}>();
	for (const vector of readVectors<InputVector>("interop/input.json")) {
		inputs.set(vector.Name, {bin: parseBin(vector.Bin), items: parseItems(vector.Items)});
	}
	return inputs;
}

function load(files: string[]): InteropArtifactCase[] {
	const inputs = loadInputs();
	const cases: InteropArtifactCase[] = [];
	for (const file of files) {
		for (const vector of readVectors<ArtifactVector>(file)) {
			const input = inputs.get(vector.Name);
			if (!input) throw new Error(`artifact row '${vector.Producer}' references unknown input '${vector.Name}'.`);
			cases.push({
				label: `${vector.Producer} — ${vector.Name}`,
				bytes: Array.from(Buffer.from(vector.Base64, "base64")),
				expected: parseEncodingInfo(vector.EncodingInfo),
				bin: input.bin,
				items: input.items,
			});
		}
	}
	return cases;
}

export const artifactFiles = ["interop/artifact-cs.json", "interop/artifact-ts.json"];

export const interopArtifactCases: InteropArtifactCase[] = load(artifactFiles);

// --- integrity: each artifact file must cover exactly the input scenarios (mirrors C# InteropIntegrityTests) ---
export const inputNames: string[] = [...loadInputs().keys()];

export function artifactNames(file: string): string[] {
	return readVectors<ArtifactVector>(file).map((vector) => vector.Name);
}
