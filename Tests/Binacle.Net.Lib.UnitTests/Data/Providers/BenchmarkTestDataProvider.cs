namespace Binacle.Net.Lib.UnitTests.Data.Providers;

internal class BenchmarkTestDataProvider : ScenarioFileTestDataProvider
{
    public BenchmarkTestDataProvider() : base($"{Constants.BasePath}/Scenarios/Benchmark.json")
    {

    }
}
