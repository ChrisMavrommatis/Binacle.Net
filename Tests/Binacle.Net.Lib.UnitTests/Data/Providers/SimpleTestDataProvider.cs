namespace Binacle.Net.Lib.UnitTests.Data.Providers;

internal class SimpleTestDataProvider : ScenarioFileTestDataProvider
{
    public SimpleTestDataProvider() : base($"{Constants.DataBasePath}/Scenarios/Simple.json")
    {

    }
}
