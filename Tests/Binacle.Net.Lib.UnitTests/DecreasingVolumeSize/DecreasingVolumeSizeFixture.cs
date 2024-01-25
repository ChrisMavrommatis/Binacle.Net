using Binacle.Net.Lib.UnitTests.DecreasingVolumeSize.Models;
using Newtonsoft.Json;

namespace Binacle.Net.Lib.UnitTests.DecreasingVolumeSize
{
    public sealed class DecreasingVolumeSizeFixture : IDisposable
    {
        public const string BaseBath = "..\\..\\..\\TestData\\DecreasingVolumeSize";

        public readonly Dictionary<string, List<Models.TestBin>> Bins;
        public readonly Dictionary<string, Scenario> Scenarios;

        public DecreasingVolumeSizeFixture()
        {
            this.Bins = new Dictionary<string, List<TestBin>>();
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

            this.Scenarios = new Dictionary<string, Scenario>();
            var scenariosDirectoryInfo = new System.IO.DirectoryInfo($"{BaseBath}\\Scenarios");
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
}
