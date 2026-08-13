// ports C#: InteropIntegrityTests
//
// Inputs and artifacts are joined by Name, so each artifact file must describe the same set of scenarios as
// input.json. A scenario added without rerunning a generator, or a stale artifact, diverges the Name sets and
// fails here before the decode test does.
import {artifactFiles, artifactNames, inputNames} from "./providers/InteropArtifacts";

describe("interop artifact files cover exactly the input scenarios", () => {
	const expected = [...inputNames].sort();
	test.each(artifactFiles)("%s", (file) => {
		expect([...artifactNames(file)].sort()).toEqual(expected);
	});
});
