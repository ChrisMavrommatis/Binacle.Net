// Ports C#: Providers/Interop/InteropVectors.cs + CSharpArtifacts.cs + TypeScriptArtifacts.cs. The C# side splits
// into a shared loader plus one provider per producer; TS keeps a single loader here. Each producer has its own
// folder (interop/cs, interop/ts) with one file per codec (raw/deflate/gzip.json), all serializing the shared
// input.json; each blob must deserialize back to it. Reads ALL of them, so TS decodes its own output AND the C#
// output, in every codec — the cross-language guarantee. Each row joins to input.json by Name. Not a *.test.ts
// file, so jest does not run it.

import {readVectors} from "../support/vectorReader";
import {parseBin, parseItems, parseHeader} from "../support/vectorParser";
import {Coordinates, Dimensions, Header} from "../../src/models";

type Item = Dimensions & Coordinates;

export type ArtifactCodec = "raw" | "deflate" | "gzip";

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
	codec: ArtifactCodec;
	bytes: number[];
	expectedHeader: Header;
	bin: Dimensions;
	items: Item[];
}

const codecs: ArtifactCodec[] = ["raw", "deflate", "gzip"];
const languages = ["cs", "ts"];

function artifactFile(lang: string, codec: ArtifactCodec): string {
	return `interop/${lang}/${codec}.json`;
}

// Every artifact file: both producers × all three codecs.
export const artifactFiles: string[] = languages.flatMap((lang) => codecs.map((codec) => artifactFile(lang, codec)));

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

// A compressed artifact carries the input's header with the compressed bit set (deflate and gzip are
// indistinguishable on the wire — §6); raw keeps the input's header as-is.
function compressedHeader(header: Header): Header {
	return new Header(
		header.version,
		true,
		header.layout,
		header.binDimensionsWidth,
		header.itemDimensionsWidth,
		header.itemCoordinatesWidth,
	);
}

// Lazy on purpose: this joins each artifact row to input.json and throws on an unknown Name. Calling it at module
// top-level would make that throw fire the moment ANYTHING imports this file — including the integrity test,
// which must run first and report "which names differ" clearly.
export function loadInteropArtifactCases(): InteropArtifactCase[] {
	const inputs = loadInputs();
	const cases: InteropArtifactCase[] = [];
	for (const lang of languages) {
		for (const codec of codecs) {
			for (const vector of readVectors<ArtifactVector>(artifactFile(lang, codec))) {
				const input = inputs.get(vector.Name);
				if (!input) throw new Error(`artifact row '${vector.Producer}' references unknown input '${vector.Name}'.`);
				cases.push({
					label: `${vector.Producer} ${codec} — ${vector.Name}`,
					codec,
					bytes: Array.from(Buffer.from(vector.Base64, "base64")),
					expectedHeader: codec === "raw" ? input.expectedHeader : compressedHeader(input.expectedHeader),
					bin: input.bin,
					items: input.items,
				});
			}
		}
	}
	return cases;
}

// --- integrity: each artifact file must cover exactly the input scenarios (mirrors C# InteropIntegrityTests) ---
export const inputNames: string[] = [...loadInputs().keys()];

export function artifactNames(file: string): string[] {
	return readVectors<ArtifactVector>(file).map((vector) => vector.Name);
}
