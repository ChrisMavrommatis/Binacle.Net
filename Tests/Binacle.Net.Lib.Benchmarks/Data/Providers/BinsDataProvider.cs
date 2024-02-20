using Binacle.Net.Lib.Benchmarks.Models;
using Newtonsoft.Json;

namespace Binacle.Net.Lib.Benchmarks.Data.Providers;

internal sealed class BinsDataProvider
{
    public readonly Dictionary<string, List<TestBin>> Bins;

    public BinsDataProvider()
    {
        this.Bins = new Dictionary<string, List<TestBin>>();
        var binsDirectoryInfo = new DirectoryInfo($"{Constants.BasePath}/Bins");
        foreach (var binCollectionFileInfo in binsDirectoryInfo.GetFiles())
        {
            var binCollectionName = Path.GetFileNameWithoutExtension(binCollectionFileInfo.Name);
            using (var sr = new StreamReader(binCollectionFileInfo.OpenRead()))
            {
                var binCollection = JsonConvert.DeserializeObject<List<TestBin>>(sr.ReadToEnd());
                this.Bins.Add(binCollectionName, binCollection);
            }
        }
    }

    public List<TestBin> GetCollection(string collectionKey)
    {
        if(!this.Bins.ContainsKey(collectionKey))
            throw new ArgumentException($"Collection with key {collectionKey} not found.")

        return this.Bins[collectionKey];
    }
}
