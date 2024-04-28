namespace Binacle.Net.Lib.UnitTests.Data.Providers;

internal class SimpleTestDataProvider : CompactScenarioFileTestDataProvider
{
	public SimpleTestDataProvider() : base($"{Constants.DataBasePath}/Scenarios/Compact/Simple.json")
	{

	}
}
