namespace Binacle.ViPaq.VectorGenerators;

// One row of test-vectors/interop/input.json — the shared input both generators read. ExpectedHeader (the
// HeaderNotation text form) is what a producer must encode under: it fixes compression, layout and the three
// widths, so the generator obeys it rather than letting the library choose. That is the only way a producer can
// emit a compressed or columnar blob, since ViPaqSerializer always writes raw, row-major, narrowest.
public sealed class InputScenario
{
	public required string Name { get; init; }
	public required string ExpectedHeader { get; init; }
	public required string Bin { get; init; }
	public required string[] Items { get; init; }
}

// One row this tool writes to artifact-cs.json (the TS tool writes the same shape to artifact-ts.json).
// Base64 is the whole serialized blob (header byte + body). The expected header lives on input.json
// (ExpectedEncodingInfo), not here — it's producer-independent and belongs with the scenario, so the artifact
// only carries the bytes the producer emitted.
public sealed class Artifact
{
	public required string Name { get; init; }
	public required string Producer { get; init; }
	public required string Base64 { get; init; }
}

// One row this tool writes to header-bytes.json — a header combo, named in HeaderNotation text form, and the
// two bytes it must pack to (PROTOCOL.md §2). The header is two bytes now, so Bytes carries both. A concrete
// class so the file's schema (field names, order) is controlled here, not in string building.
public sealed class HeaderByteVector
{
	public required string Header { get; init; }
	public required string[] Bytes { get; init; }
}
