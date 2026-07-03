// Ports C#: Artifact (Contracts.cs). The row this tool writes to artifact-ts.json — a class so the file's
// serialized schema (field names and order) is controlled here, mirroring the C# concrete class. Base64 is
// the whole serialized blob (header byte + body); EncodingInfo pins what byte 0 must decode to.
export class Artifact {
	constructor(
		public readonly Name: string,
		public readonly Producer: string,
		public readonly EncodingInfo: string,
		public readonly Base64: string,
	) {}
}
