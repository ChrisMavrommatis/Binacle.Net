// Ports C#: Artifact (Contracts.cs). The row this tool writes to artifact-ts.json — a class so the file's
// serialized schema (field names and order) is controlled here, mirroring the C# concrete class. Base64 is the
// whole serialized blob (the two header bytes + body). The expected header lives on input.json (ExpectedHeader),
// not here — it's producer-independent, so the artifact only carries the bytes this producer emitted.
export class Artifact {
	constructor(
		public readonly Name: string,
		public readonly Producer: string,
		public readonly Base64: string,
	) {}
}
