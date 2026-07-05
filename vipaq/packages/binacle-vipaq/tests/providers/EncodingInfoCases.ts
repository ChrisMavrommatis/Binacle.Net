// Ports C#: Providers/EncodingInfoByteProvider.cs. Every EncodingInfo combo (all 256) and the header byte
// it packs to. `byte` is an independent golden — the parser building `info` never sees it, so the test is a
// real check, not a round-trip. Consumed by tests/utils/encodingInfo.test.ts.
// Not a *.test.ts file, so jest does not run it.

import {readVectors} from "../support/vectorReader";
import {parseEncodingInfo, parseByte} from "../support/vectorParser";
import {EncodingInfo} from "../../src/models";

// Raw encoding-info-bytes.json row: the EncodingInfo string and the header byte it packs to.
interface Vector {
	EncodingInfo: string;
	Byte: string;
}

// A resolved scenario (mirrors the C# provider's Scenario record): the parsed EncodingInfo, its golden
// header byte, and the EncodingInfo string as the test label.
export interface Scenario {
	encodingInfo: string;
	info: EncodingInfo;
	byte: number;
}

// Reads the vectors row by row and resolves each into a Scenario. Runs once, when this module is imported.
function load(file: string): Scenario[] {
	const scenarios: Scenario[] = [];
	for (const vector of readVectors<Vector>(file)) {
		scenarios.push({
			encodingInfo: vector.EncodingInfo,
			info: parseEncodingInfo(vector.EncodingInfo),
			byte: parseByte(vector.Byte),
		});
	}
	return scenarios;
}

export const encodingInfoCases: Scenario[] = load("encoding-info-bytes.json");
