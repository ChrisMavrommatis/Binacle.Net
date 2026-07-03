namespace Binacle.ViPaq.Generators;

// One row of test-vectors/interop/input.json — the shared input both generators read.
public sealed class InputScenario
{
	public required string Name { get; init; }
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

// One row this tool writes to encoding-info-bytes.json — a header combo and the byte it must pack to.
// A concrete class so the file's schema (field names, order) is controlled here, not in string building.
public sealed class EncodingInfoByteVector
{
	public required string EncodingInfo { get; init; }
	public required string Byte { get; init; }
}
