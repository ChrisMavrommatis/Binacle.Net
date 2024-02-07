using Binacle.Net.Lib.Abstractions.Strategies;
using Newtonsoft.Json;

namespace Binacle.Net.Lib.UnitTests.DecreasingVolumeSize;

public sealed class DecreasingVolumeSizeFixture : IDisposable
{
    public const string BaseBath = "..\\..\\..\\DecreasingVolumeSize\\Data";

    public readonly Dictionary<string, List<Models.TestBin>> Bins;
    public readonly Dictionary<string, Models.Scenario> Scenarios;

    private readonly List<Type> strategyTypes;

    public DecreasingVolumeSizeFixture()
    {
        this.Bins = new Dictionary<string, List<Models.TestBin>>();
        var binsDirectoryInfo = new System.IO.DirectoryInfo($"{BaseBath}\\Bins");
        foreach (var binCollectionFileInfo in binsDirectoryInfo.GetFiles())
        {
            var binCollectionName = Path.GetFileNameWithoutExtension(binCollectionFileInfo.Name);
            using (var sr = new StreamReader(binCollectionFileInfo.OpenRead()))
            {
                var binCollection = JsonConvert.DeserializeObject<List<Models.TestBin>>(sr.ReadToEnd());
                this.Bins.Add(binCollectionName, binCollection);
            }
        }

        this.Scenarios = new Dictionary<string, Models.Scenario>();
        var scenariosDirectoryInfo = new System.IO.DirectoryInfo($"{BaseBath}\\Scenarios");
        foreach (var scenarioCollectionFileInfo in scenariosDirectoryInfo.GetFiles())
        {
            var scenarioCollectionName = Path.GetFileNameWithoutExtension(scenarioCollectionFileInfo.Name);
            using (var sr = new StreamReader(scenarioCollectionFileInfo.OpenRead()))
            {
                var scenarios = JsonConvert.DeserializeObject<List<Models.Scenario>>(sr.ReadToEnd());
                foreach (var scenario in scenarios)
                {
                    this.Scenarios.Add($"{scenarioCollectionName}_{scenario.Name}", scenario);
                }
            }
        }

        this.strategyTypes = new List<Type>();
        this.RegisterStrategyType<Binacle.Net.Lib.Strategies.DecreasingVolumeSize_v1>();
        this.RegisterStrategyType<Binacle.Net.Lib.Strategies.DecreasingVolumeSize_v2>();
    }

    public void Dispose()
    {

    }

    private void RegisterStrategyType<TStrategy>()
        where TStrategy : class, IBinFittingStrategy
    {
        this.strategyTypes.Add(typeof(TStrategy));
    }

    public IEnumerable<IBinFittingStrategy> GetRegisteredStrategies()
    {
        foreach (var strategyType in this.strategyTypes)
        {
            yield return (IBinFittingStrategy)Activator.CreateInstance(strategyType, true)!;
        }
    }
}

