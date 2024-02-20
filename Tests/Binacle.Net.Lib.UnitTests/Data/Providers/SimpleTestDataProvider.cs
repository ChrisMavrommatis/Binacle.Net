using Binacle.Net.Lib.UnitTests.FirstFitDecreasing;

namespace Binacle.Net.Lib.UnitTests.Data.Providers;

internal class SimpleTestDataProvider : ScenarioFileTestDataProvider
{
    public SimpleTestDataProvider() : base($"{Constants.BasePath}/Scenarios/Simple.json")
    {

    }
}
