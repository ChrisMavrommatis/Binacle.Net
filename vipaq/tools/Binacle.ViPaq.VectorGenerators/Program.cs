using Binacle.ViPaq.VectorGenerators;

// Regenerates every committed C# ViPaq test vector, which both test suites read. Takes no arguments on purpose:
// a regen always runs every generator, so it cannot half-run and leave the vectors inconsistent. Output is
// deterministic, so a no-change re-run is byte-identical.
//
// Add a vector by writing an IVectorGenerator and dropping it in the list below. The TS side has its own
// generator for artifact-ts.json; `npm run regen:interop` runs both so the interop halves cannot drift.

IVectorGenerator[] generators =
[
	new InteropArtifactGenerator(),
	new HeaderBytesGenerator(),
];

foreach (var generator in generators)
{
	generator.Generate();
}
