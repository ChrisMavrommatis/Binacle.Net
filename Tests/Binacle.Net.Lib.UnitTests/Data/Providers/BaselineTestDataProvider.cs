namespace Binacle.Net.Lib.UnitTests.Data.Providers;

internal class BaselineTestDataProvider : ScenarioFileTestDataProvider
{
	public BaselineTestDataProvider() : base($"{Constants.DataBasePath}/Scenarios/Baseline.json")
	{

	}
}
