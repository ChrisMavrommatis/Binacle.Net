using Binacle.Net.TestsKernel.Data.Providers.BinaryDecision;

namespace Binacle.Net.Lib.UnitTests.Data.Providers.BinaryDecision;

internal sealed class FittingAlgorithmsBaselineScenariosProvider: BaselineScenarioTestDataProvider
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
