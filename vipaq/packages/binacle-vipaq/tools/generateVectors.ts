import {generateInteropArtifact} from "./interopArtifactGenerator";

// Regenerates every committed TS ViPaq test vector — files that both test suites read. Mirrors the C# tool's
// Program: no arguments, runs each generator in the list so a regen can't half-run. Generators are plain
// async functions (no generator classes); add one by writing it and dropping it in the list. The serialized
// output shapes are classes (see Artifact) so the file schema is controlled. Run via `npm run generate:interop`
// (or `npm run regen:interop`, which runs the C# generators first).

const generators: Array<() => Promise<void>> = [
	generateInteropArtifact,
];

async function main(): Promise<void> {
	for (const generate of generators) {
		await generate();
	}
}

main().catch((error) => {
	console.error(error);
	process.exit(1);
});
