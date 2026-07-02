namespace Binacle.ViPaq.Generators;

// One row of test-vectors/interop/input.json — the shared input both generators read.
public sealed class InputScenario
{
	public required string Name { get; init; }
	public required string Bin { get; init; }
	public required string[] Items { get; init; }
}

// One row this tool writes to artifact-cs.json (the TS tool writes the same shape to artifact-ts.json).
// Base64 is the whole serialized blob (header byte + body); EncodingInfo pins what byte 0 must decode to.
public sealed class Artifact
{
	public required string Name { get; init; }
	public required string Producer { get; init; }
	public required string EncodingInfo { get; init; }
	public required string Base64 { get; init; }
}
