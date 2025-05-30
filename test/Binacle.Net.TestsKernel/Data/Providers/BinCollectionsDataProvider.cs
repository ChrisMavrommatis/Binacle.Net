using System.Text.Json;
using Binacle.Net.TestsKernel.Abstractions.Models;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Net.TestsKernel.Data.Providers;

public class BinCollectionsDataProvider : DataCollections<List<TestBin>>
{
	private const string binCollectionsKey = "BinCollections";

	public BinCollectionsDataProvider()
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
				var binCollection = JsonSerializer.Deserialize<List<TestBin>>(sr.ReadToEnd());
				if (binCollection is not null)
				{
					collections.Add(binCollectionName.ToLower(), binCollection);
				}
			}
		}

		return collections;
	}
}
