using Binacle.ViPaq.VectorGenerators;

// Regenerates every committed C# ViPaq test vector — files that BOTH test suites read. Takes no arguments on
// purpose: a regen always runs every generator, so it can't half-run and leave the vectors inconsistent.
// Output is deterministic, so a no-change re-run is byte-identical (no git noise). Add a new vector by writing
// an IVectorGenerator and dropping it in the list below. Run by hand when the inputs or the spec change (see
// the cross-language plan). The TS side has its own generator for artifact-ts.json (npm run generate:interop);
// "npm run regen:interop" runs both so the interop halves can't drift.

IVectorGenerator[] generators =
[
	new InteropArtifactGenerator(),
	new HeaderBytesGenerator(),
];

foreach (var generator in generators)
{
	generator.Generate();
}
