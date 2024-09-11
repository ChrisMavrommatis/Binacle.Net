using Binacle.Net.TestsKernel.Providers;

namespace Binacle.Net.TestsKernel.Models;

public abstract class BinScenarioBase
{
	private readonly string binString;

	private TestBin? bin;

	public BinScenarioBase(string binString)
	{
		this.binString = binString;
	}

	public TestBin GetTestBin(BinCollectionsTestDataProvider provider)
	{
		if (this.bin is not null)
		{
			return this.bin;
		}

		var binParts = this.binString.Split("::");
		if (binParts.Length != 2)
		{
			throw new InvalidOperationException($"Unexpected Bin format {this.binString}. Bin should be 'CollectionKey::Bin' or 'Raw::LxWxH'");
		}
		if (binParts[0] == "Raw")
		{
			var dimensions = Helpers.DimensionHelper.ParseFromCompactString(binParts[1]);
			this.bin = new TestBin(binParts[1], dimensions);
		}
		else
		{
			var collectionKey = binParts[0];
			var collection = provider.GetCollection(collectionKey);
			if (collection is null)
			{
				throw new InvalidOperationException($"Collection {collectionKey} not found");
			}
			var bin = collection.FirstOrDefault(x => x.ID == binParts[1]);
			if (bin is null)
			{
				throw new InvalidOperationException($"Bin {binParts[1]} not found in collection {collectionKey}");
			}
			this.bin = bin;
		}

		return this.bin;
	}
}

