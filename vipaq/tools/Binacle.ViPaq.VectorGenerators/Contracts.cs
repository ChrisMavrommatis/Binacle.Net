namespace Binacle.ViPaq.VectorGenerators;

// One row of test-vectors/interop/input.json, the shared input both generators read. ExpectedHeader fixes
// compression, layout and the three widths, and the generator obeys it rather than letting the library choose
// - the only way to emit a compressed or columnar blob, since ViPaqSerializer always writes raw, row-major,
// narrowest.
public sealed class InputScenario
{
	public required string Name { get; init; }
	public required string ExpectedHeader { get; init; }
	public required string Bin { get; init; }
	public required string[] Items { get; init; }
}

// One row this tool writes to artifact-cs.json; the TS tool writes the same shape to artifact-ts.json. The
// expected header lives on input.json, not here: it is producer-independent, so the artifact only carries the
// bytes the producer emitted.
public sealed class Artifact
{
	public required string Name { get; init; }
	public required string Producer { get; init; }
	public required string Base64 { get; init; }
}

// One row this tool writes to header-bytes.json: a header combo in HeaderNotation text form and the two bytes
// it must pack to (PROTOCOL.md §2). A concrete class, so the file's schema lives here, not in string building.
public sealed class HeaderByteVector
{
	public required string Header { get; init; }
	public required string[] Bytes { get; init; }
}
