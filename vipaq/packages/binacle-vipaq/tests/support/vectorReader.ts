// Ports C#: Providers/VectorReader.cs. Reads a shared vector file from vipaq/test-vectors/ into its rows. C#
// embeds the same files as resources, so neither side can drift on the wire format. Edit a case in the JSON.
// Not a *.test.ts file, so jest does not run it.

import * as fs from "fs";
import * as path from "path";

// Resolved against this file's location so it works regardless of the jest working directory.
const root = path.join(__dirname, "../../../../test-vectors");

// Reads a vector file (e.g. "serialization/exact-bytes.json" or "protocol/little-endian/uint16.json") into its rows.
export function readVectors<T>(fileName: string): T[] {
	return JSON.parse(fs.readFileSync(path.join(root, fileName), "utf8")) as T[];
}
