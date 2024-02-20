using Binacle.Net.Lib.UnitTests.Models;
using Newtonsoft.Json;
using System.Collections;

namespace Binacle.Net.Lib.UnitTests.Data.Providers;

internal abstract class ScenarioFileTestDataProvider : IEnumerable<object[]>
{
    private readonly Dictionary<string, Scenario> scenarios;

    public ScenarioFileTestDataProvider(string filePath)
    {
        scenarios = new Dictionary<string, Scenario>();
        var scenarioFileInfo = new FileInfo(filePath);
        using (var sr = new StreamReader(scenarioFileInfo.OpenRead()))
        {
            var scenarios = JsonConvert.DeserializeObject<List<Scenario>>(sr.ReadToEnd());
            foreach (var scenario in scenarios)
            {
                this.scenarios.Add(scenario.Name, scenario);
            }
        }
    }

    public IEnumerator<object[]> GetEnumerator()
    {
        foreach (var scenario in scenarios.Values)
        {
            yield return new object[] { scenario };
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}