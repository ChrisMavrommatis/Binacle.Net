using Binacle.Net.Lib.Tests.Models;
using Binacle.Net.Lib.UnitTests.Models;
using Newtonsoft.Json;

namespace Binacle.Net.Lib.UnitTests.FirstFitDecreasing;

public sealed class FirstFitDecreasingFixture : IDisposable
{
    public readonly Dictionary<string, List<TestBin>> Bins;
    public readonly Dictionary<string, Scenario> Scenarios;

    public FirstFitDecreasingFixture()
    {
        this.Bins = new Dictionary<string, List<TestBin>>();
        var binsDirectoryInfo = new System.IO.DirectoryInfo($"{Data.Constants.BasePath}/Bins");
        foreach (var binCollectionFileInfo in binsDirectoryInfo.GetFiles())
        {
            var binCollectionName = Path.GetFileNameWithoutExtension(binCollectionFileInfo.Name);
            using (var sr = new StreamReader(binCollectionFileInfo.OpenRead()))
            {
                var binCollection = JsonConvert.DeserializeObject<List<TestBin>>(sr.ReadToEnd());
                this.Bins.Add(binCollectionName, binCollection);
            }
        }

        this.Scenarios = new Dictionary<string, Scenario>();
        var scenariosDirectoryInfo = new System.IO.DirectoryInfo($"{Data.Constants.BasePath}/Scenarios");
        foreach (var scenarioCollectionFileInfo in scenariosDirectoryInfo.GetFiles())
        {
            var scenarioCollectionName = Path.GetFileNameWithoutExtension(scenarioCollectionFileInfo.Name);
            using (var sr = new StreamReader(scenarioCollectionFileInfo.OpenRead()))
            {
                var scenarios = JsonConvert.DeserializeObject<List<Scenario>>(sr.ReadToEnd());
                foreach (var scenario in scenarios)
                {
                    this.Scenarios.Add($"{scenarioCollectionName}_{scenario.Name}", scenario);
                }
            }
        }
    }

    public void Dispose()
    {

    }
  
}

