// Ports C#: Artifact (Contracts.cs). The row this tool writes to artifact-ts.json. A class, so the file's
// serialized schema lives here. The expected header lives on input.json, not here: it is producer-independent,
// so the artifact only carries the bytes this producer emitted.
export class Artifact {
	constructor(
		public readonly Name: string,
		public readonly Producer: string,
		public readonly Base64: string,
	) {}
}
