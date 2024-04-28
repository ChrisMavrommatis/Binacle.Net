namespace Binacle.Net.Lib.UnitTests.Data.Providers;

internal class BaselineTestDataProvider : CompactScenarioFileTestDataProvider
{
	public BaselineTestDataProvider() : base($"{Constants.DataBasePath}/Scenarios/Compact/Baseline.json")
	{

	}
}
