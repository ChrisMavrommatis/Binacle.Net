using Binacle.Net.Lib.UnitTests.FirstFitDecreasing;

namespace Binacle.Net.Lib.UnitTests.Data.Providers;

internal class ComplexTestDataProvider : ScenarioFileTestDataProvider
{
    public ComplexTestDataProvider() : base($"{Constants.BasePath}/Scenarios/Complex.json")
    {

    }
}
