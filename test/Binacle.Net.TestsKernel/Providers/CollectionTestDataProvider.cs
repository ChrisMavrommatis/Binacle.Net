using System.Collections.ObjectModel;

namespace Binacle.Net.TestsKernel.Providers;

public abstract class CollectionTestDataProvider<T>
	where T : class, new()
{
	private Dictionary<string, T> collections;

	public ReadOnlyDictionary<string, T> Collections => this.collections.AsReadOnly();

	protected CollectionTestDataProvider(string solutionRootPath)
	{
		this.collections = this.InitializeCollections(solutionRootPath);
	}

	protected abstract Dictionary<string, T> InitializeCollections(string solutionRootPath);

	public virtual T GetCollection(string collectionKey)
	{
		var normalizedKey = collectionKey.ToLower();

		if (!this.collections.ContainsKey(normalizedKey))
			throw new ArgumentException($"Collection with key {normalizedKey} not found.");

		return this.collections[normalizedKey];
	}
}


