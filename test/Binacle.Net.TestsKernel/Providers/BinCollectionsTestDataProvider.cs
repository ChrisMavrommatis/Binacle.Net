using Binacle.Net.TestsKernel.Data;
using Binacle.Net.TestsKernel.Models;
using Newtonsoft.Json;

namespace Binacle.Net.TestsKernel.Providers;

public class BinCollectionsTestDataProvider : CollectionTestDataProvider<List<TestBin>>
{
	private const string binCollectionsKey = "BinCollections";

	public BinCollectionsTestDataProvider()
	{
	}

	protected override Dictionary<string, List<TestBin>> InitializeCollections()
	{
		var collections = new Dictionary<string, List<TestBin>>();

		foreach (var binCollectionFile in EmbeddedResourceFileProvider.GetFiles(binCollectionsKey))
		{
			var binCollectionName = Path.GetFileNameWithoutExtension(binCollectionFile.Name);
			using (var sr = new StreamReader(binCollectionFile.OpenRead()))
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
