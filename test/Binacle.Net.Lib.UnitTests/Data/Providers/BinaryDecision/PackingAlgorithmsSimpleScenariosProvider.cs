using Binacle.Net.TestsKernel.Data.Providers.BinaryDecision;

namespace Binacle.Net.Lib.UnitTests.Data.Providers.BinaryDecision;

internal sealed class PackingAlgorithmsSimpleScenariosProvider: SimpleScenarioTestDataProvider
{
	public override IEnumerator<object[]> GetEnumerator()
	{
		foreach (var (algorithmKey, algorithm)  in AlgorithmsUnderTest.PackingAlgorithms)
		{
			foreach (var scenario in this.Scenarios)
			{
				yield return new object[] { algorithmKey, scenario };
			}
		}
		
	}
}
