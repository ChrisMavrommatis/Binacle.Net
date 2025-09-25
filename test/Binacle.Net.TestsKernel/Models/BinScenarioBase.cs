using Binacle.Net.TestsKernel.Data.Providers;
using Binacle.Net.TestsKernel.Helpers;

namespace Binacle.Net.TestsKernel.Models;

public abstract class BinScenarioBase
{
	private TestBin? bin;
	private string? binCollectionKey;
	private string? binFromCollection;
	public BinScenarioBase(string binString)
	{
		this.ParseBinString(binString);
	}

	private void ParseBinString(string binString)
	{
		var binParts = binString.Split("::");
		if (binParts.Length != 2)
		{
			throw new InvalidOperationException($"Unexpected Bin format {binString}. Bin should be 'CollectionKey::Bin' or 'Raw::LxWxH'");
		}
		if (binParts[0] == "Raw")
		{
			var dimensions = DimensionHelper.ParseFromCompactString(binParts[1]);
			this.bin = new TestBin(binParts[1], dimensions);
			return;
		}
		this.binCollectionKey = binParts[0];
		this.binFromCollection = binParts[1];

		if (string.IsNullOrWhiteSpace(this.binCollectionKey)
			|| string.IsNullOrWhiteSpace(this.binFromCollection))
		{
			throw new InvalidOperationException($"Unexpected Bin format {binString}. Bin should be 'CollectionKey::Bin' or 'Raw::LxWxH'");
		}

	}
	public string GetBinCollectionKey()
	{
		if (string.IsNullOrWhiteSpace(this.binCollectionKey)
			|| string.IsNullOrWhiteSpace(this.binFromCollection))
		{
			throw new InvalidOperationException($"Scenario was not initialized from a collection");
		}

		return this.binCollectionKey;
	}

	public TestBin GetTestBin(BinCollectionsDataProvider provider)
	{
		if (this.bin is not null)
		{
			return this.bin;
		}
		var collection = provider.GetCollection(this.binCollectionKey!);

		if (collection is null)
		{
			throw new InvalidOperationException($"Collection {this.binCollectionKey} not found");
		}
		var bin = collection.FirstOrDefault(x => x.ID == this.binFromCollection);
		if (bin is null)
		{
			throw new InvalidOperationException($"Bin '{this.binFromCollection}' not found in collection '{this.binCollectionKey}'");
		}
		this.bin = bin;

		return this.bin;
	}
}
