// Ports C#: Providers/Interop/InteropVectors.cs + CSharpArtifacts.cs + TypeScriptArtifacts.cs. The C# side
// splits into a shared loader plus one provider per file; TS keeps a single file-list loader here. The two
// interop artifacts (interop/artifact-cs.json, interop/artifact-ts.json) both serialize the shared
// input.json; each blob must deserialize back to it. Reads BOTH files so TS decodes its own output AND the
// C# output — the cross-language guarantee. Each row joins to input.json by Name. Not a *.test.ts file, so
// jest does not run it.

import {readVectors} from "../support/vectorReader";
import {parseBin, parseItems, parseHeader} from "../support/vectorParser";
import {Coordinates, Dimensions, Header} from "../../src/models";

type Item = Dimensions & Coordinates;

interface InputVector {
	Name: string;
	ExpectedHeader: string;
	Bin: string;
	Items: string[];
}

interface ArtifactVector {
	Name: string;
	Producer: string;
	Base64: string;
}

// ExpectedHeader lives on the input (producer-independent, spec-determined), so the header pin checks a declared
// value instead of one echoed back from the generator's own output.
type Input = {expectedHeader: Header; bin: Dimensions; items: Item[]};

export interface InteropArtifactCase {
	label: string;
	bytes: number[];
	expectedHeader: Header;
	bin: Dimensions;
	items: Item[];
}

function loadInputs(): Map<string, Input> {
	const inputs = new Map<string, Input>();
	for (const vector of readVectors<InputVector>("interop/input.json")) {
		inputs.set(vector.Name, {
			expectedHeader: parseHeader(vector.ExpectedHeader),
			bin: parseBin(vector.Bin),
			items: parseItems(vector.Items),
		});
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
				expectedHeader: input.expectedHeader,
				bin: input.bin,
				items: input.items,
			});
		}
	}
	return cases;
}

export const artifactFiles = ["interop/artifact-cs.json", "interop/artifact-ts.json"];

// Lazy on purpose: load() joins each artifact row to input.json and throws on an unknown Name. Calling it
// here at module top-level would make that throw fire the moment ANYTHING imports this file — including the
// integrity test, which must run first and report "which names differ" clearly. interop.test.ts calls this
// in its test.each; the integrity test imports only the join-free names below and never triggers the join.
export function loadInteropArtifactCases(): InteropArtifactCase[] {
	return load(artifactFiles);
}

// --- integrity: each artifact file must cover exactly the input scenarios (mirrors C# InteropIntegrityTests) ---
export const inputNames: string[] = [...loadInputs().keys()];

export function artifactNames(file: string): string[] {
	return readVectors<ArtifactVector>(file).map((vector) => vector.Name);
}
