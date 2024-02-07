using Binacle.Net.Lib.UnitTests.DecreasingVolumeSize.Models;
using Newtonsoft.Json;
using System.Collections;

namespace Binacle.Net.Lib.UnitTests.DecreasingVolumeSize.Data;

internal abstract class ScenarioFileTestDataProvider : IEnumerable<object[]>
{
    private readonly Dictionary<string, Scenario> scenarios;

    public ScenarioFileTestDataProvider(string filePath)
    {
        this.scenarios = new Dictionary<string, Models.Scenario>();
        var scenarioFileInfo = new System.IO.FileInfo(filePath);
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
        foreach (var scenario in this.scenarios.Values)
        {
            yield return new object[] { scenario };
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}