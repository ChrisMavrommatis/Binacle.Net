using Binacle.Net.TestsKernel.Data.Providers.BinaryDecision;

namespace Binacle.Lib.UnitTests.Data.Providers.BinaryDecision;

public sealed class AlgorithmsSimpleScenariosProvider: SimpleScenarioDataProvider
{
	public override IEnumerator<object[]> GetEnumerator()
	{
		foreach (var (algorithmKey, algorithm)  in AlgorithmsUnderTest.All)
		{
			foreach (var scenario in this.Scenarios)
			{
				yield return new object[] { algorithmKey, scenario };
			}
		}
		
	}
}
