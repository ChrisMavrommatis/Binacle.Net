// ports C#: InteropIntegrityTests
//
// Inputs and artifacts are joined by Name, so EACH artifact file must describe the exact same set of
// scenarios as input.json. If a scenario is added to input.json but a generator wasn't rerun (or a stale
// artifact is left behind), the Name sets diverge and this fails before the decode test does.
import {artifactFiles, artifactNames, inputNames} from "./providers/InteropArtifacts";

describe("interop artifact files cover exactly the input scenarios", () => {
	const expected = [...inputNames].sort();
	test.each(artifactFiles)("%s", (file) => {
		expect([...artifactNames(file)].sort()).toEqual(expected);
	});
});
