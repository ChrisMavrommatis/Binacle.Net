using Binacle.Net.Lib.UnitTests.FirstFitDecreasing;

namespace Binacle.Net.Lib.UnitTests.Data.Providers;

internal class BaselineTestDataProvider : ScenarioFileTestDataProvider
{
    public BaselineTestDataProvider() : base($"{Constants.BasePath}/Scenarios/Baseline.json")
    {

    }
}
