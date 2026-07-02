using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// Sanity guard for the interop vectors. Inputs and artifacts are joined by Name, so the two files must
// describe the exact same set of scenarios — every Name in input.json present in each artifact file, and
// no artifact carrying a Name that input.json doesn't have. If someone adds a scenario to input.json but
// forgets to rerun the generator (or leaves a stale artifact behind), the Name sets diverge and this
// fails with a clear "which names differ" — before the decode tests fail in a murkier way.
[Trait("Result Tests", "Ensures results are as expected")]
public class InteropIntegrityTests
{
	[Fact]
	public void CSharp_Artifacts_Cover_Exactly_The_Input_Scenarios()
	{
		// ShouldBe with ignoreOrder is a set compare — it fails if either side has a Name the other lacks.
		InteropProvider.CSharpArtifactNames.ShouldBe(InteropProvider.InputNames, ignoreOrder: true);
	}
}
