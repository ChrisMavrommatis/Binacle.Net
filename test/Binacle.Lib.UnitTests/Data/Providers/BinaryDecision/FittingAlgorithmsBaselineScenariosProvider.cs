using Binacle.Net.TestsKernel.Data.Providers.BinaryDecision;

namespace Binacle.Lib.UnitTests.Data.Providers.BinaryDecision;

internal sealed class FittingAlgorithmsBaselineScenariosProvider: BaselineScenarioDataProvider
{
	public override IEnumerator<object[]> GetEnumerator()
	{
		foreach (var (algorithmKey, algorithm) in AlgorithmsUnderTest.FittingAlgorithms)
		{
			foreach (var scenario in this.Scenarios)
			{
				yield return new object[] { algorithmKey, scenario };
			}
		}
		
	}
}
