using Binacle.Net.TestsKernel.Models;
using Newtonsoft.Json;

namespace Binacle.Net.TestsKernel.Providers;

public class BinCollectionsTestDataProvider : CollectionTestDataProvider<List<TestBin>>
{
	public BinCollectionsTestDataProvider(string solutionRootBasePath)
	{
		var binsDirectoryInfo = new DirectoryInfo(
			Path.Combine(solutionRootBasePath, Constants.DataBasePathRoot, "BinCollections")
			);

		foreach (var binCollectionFileInfo in binsDirectoryInfo.GetFiles())
		{
			var binCollectionName = Path.GetFileNameWithoutExtension(binCollectionFileInfo.Name);
			using (var sr = new StreamReader(binCollectionFileInfo.OpenRead()))
			{
				var binCollection = JsonConvert.DeserializeObject<List<TestBin>>(sr.ReadToEnd());
				this.Collections.Add(binCollectionName, binCollection);
			}
		}
	}

}
