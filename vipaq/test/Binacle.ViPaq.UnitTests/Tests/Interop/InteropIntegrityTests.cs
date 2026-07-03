using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// Sanity guard for the interop vectors. Inputs and artifacts are joined by Name, so EACH artifact file
// (artifact-cs.json and artifact-ts.json) must describe the exact same set of scenarios as input.json —
// every input Name present in the file, and no artifact carrying a Name input.json doesn't have. If
// someone adds a scenario to input.json but forgets to rerun a generator (or leaves a stale artifact),
// the Name sets diverge and this fails with a clear "which names differ" — before the decode tests fail
// in a murkier way. One method per producer.
[Trait("Sanity Tests", "Ensures the tests are configured correctly")]
public class InteropIntegrityTests
{
	[Fact]
	public void CSharp_Artifacts_Cover_Exactly_The_Input_Scenarios()
		=> InteropVectors.ReadNames(InteropFiles.CSharpArtifact).ShouldBe(InteropVectors.InputNames, ignoreOrder: true);

	[Fact]
	public void TypeScript_Artifacts_Cover_Exactly_The_Input_Scenarios()
		=> InteropVectors.ReadNames(InteropFiles.TypeScriptArtifact).ShouldBe(InteropVectors.InputNames, ignoreOrder: true);
}
