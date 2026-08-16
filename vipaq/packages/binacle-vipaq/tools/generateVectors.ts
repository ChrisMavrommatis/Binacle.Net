import {generateInteropArtifact} from "./interopArtifactGenerator";

// Regenerates every committed TS ViPaq test vector. Mirrors the C# tool's Program: no arguments, runs every
// generator in the list, so a regen cannot half-run. Add a generator by writing an async function and dropping
// it in the list. `just regen vipaq-interop-vectors` runs the C# half first.

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
