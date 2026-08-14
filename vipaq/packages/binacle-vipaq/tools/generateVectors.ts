import {generateInteropArtifact} from "./interopArtifactGenerator";

// Regenerates every committed TS ViPaq test vector. Mirrors the C# tool's Program: no arguments, runs every
// generator in the list, so a regen cannot half-run. Add a generator by writing an async function and dropping
// it in the list. Run via `npm run generate:interop`, or `npm run regen:interop` to run the C# generators
// first.

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
