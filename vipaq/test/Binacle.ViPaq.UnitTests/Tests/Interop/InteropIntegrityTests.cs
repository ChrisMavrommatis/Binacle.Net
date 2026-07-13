using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// Sanity guard for the interop vectors. Inputs and artifacts are joined by Name, so EVERY artifact file — both
// producers × all three codecs — must describe the exact same set of scenarios as input.json. If someone adds a
// scenario to input.json but forgets to rerun a generator (or leaves a stale artifact), the Name sets diverge and
// this fails with a clear "which names differ" — before the decode test fails in a murkier way.
[Trait("Sanity Tests", "Ensures the tests are configured correctly")]
public class InteropIntegrityTests
{
	public static IEnumerable<object[]> ArtifactFiles
		=> InteropFiles.All().Select(file => new object[] { file });

	[Theory]
	[MemberData(nameof(ArtifactFiles))]
	public void Artifacts_Cover_Exactly_The_Input_Scenarios(string fileName)
		=> InteropVectors.ReadNames(fileName).ShouldBe(InteropVectors.InputNames, ignoreOrder: true);
}
