using Binacle.Net.TestsKernel.Models;
using Newtonsoft.Json;

namespace Binacle.Net.TestsKernel.Providers;

public class BinCollectionsTestDataProvider : CollectionTestDataProvider<List<TestBin>>
{
	private const string binCollectionsDirectory = "BinCollections";


	public BinCollectionsTestDataProvider(string solutionRootPath) :base(solutionRootPath)
	{
	}

	protected override Dictionary<string, List<TestBin>> InitializeCollections(string solutionRootPath)
	{
		var collections = new Dictionary<string, List<TestBin>>();

		var binsDirectoryInfo = new DirectoryInfo(
			Path.Combine(solutionRootPath, Constants.DataBasePathRoot, binCollectionsDirectory)
			);

		foreach (var binCollectionFileInfo in binsDirectoryInfo.GetFiles())
		{
			var binCollectionName = Path.GetFileNameWithoutExtension(binCollectionFileInfo.Name);
			using (var sr = new StreamReader(binCollectionFileInfo.OpenRead()))
			{
				var binCollection = JsonConvert.DeserializeObject<List<TestBin>>(sr.ReadToEnd());
				if (binCollection is not null)
				{
					collections.Add(binCollectionName.ToLower(), binCollection);
				}
			}
		}

		return collections;
	}
}
