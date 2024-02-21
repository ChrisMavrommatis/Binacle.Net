using Binacle.Net.Lib.Tests.Models;
using Newtonsoft.Json;

namespace Binacle.Net.Lib.Benchmarks.Data;

internal sealed class BenchmarksDataProvider
{
    private readonly Dictionary<string, List<TestBin>> bins;
    
    public BenchmarksDataProvider(string basePath)
    {
        this.bins = new Dictionary<string, List<TestBin>>();
        var binsDirectoryInfo = new DirectoryInfo($"{basePath}/Data/Bins");
        foreach (var binCollectionFileInfo in binsDirectoryInfo.GetFiles())
        {
            var binCollectionName = Path.GetFileNameWithoutExtension(binCollectionFileInfo.Name);
            using (var sr = new StreamReader(binCollectionFileInfo.OpenRead()))
            {
                var binCollection = JsonConvert.DeserializeObject<List<TestBin>>(sr.ReadToEnd());

                if (binCollection != null)
                {
                    this.bins.Add(binCollectionName, binCollection);
                }
            }
        }
    }

    public List<TestBin> GetBinCollection(string collectionKey)
    {
        if (!this.bins.ContainsKey(collectionKey))
            throw new ArgumentException($"Collection with key {collectionKey} not found.");

        return this.bins[collectionKey];
    }
}

