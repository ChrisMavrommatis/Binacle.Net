using Binacle.Net.TestsKernel.Models;
using Newtonsoft.Json;

namespace Binacle.Net.TestsKernel.Data.Providers;

public class BinTestDataProvider
{
	public readonly Dictionary<string, List<TestBin>> Bins;

	public BinTestDataProvider(string solutionRootBasePath)
	{
		this.Bins = new Dictionary<string, List<TestBin>>();

		var binsDirectoryInfo = new System.IO.DirectoryInfo(
			Path.Combine(solutionRootBasePath, Constants.DataBasePathRoot, "Bins")
			);

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

	public List<TestBin> GetBinCollection(string collectionKey)
	{
		if (!this.Bins.ContainsKey(collectionKey))
			throw new ArgumentException($"Collection with key {collectionKey} not found.");

		return this.Bins[collectionKey];
	}
}
