namespace Binacle.Net.TestsKernel.Providers;

public abstract class CollectionTestDataProvider<T>
	where T : class, new()
{
	public Dictionary<string, T> Collections { get; }

	protected CollectionTestDataProvider()
	{
		this.Collections = new Dictionary<string, T>();
	}

	public virtual T GetCollection(string collectionKey)
	{
		var normalizedKey = collectionKey.ToLower();

		if (!this.Collections.ContainsKey(normalizedKey))
			throw new ArgumentException($"Collection with key {normalizedKey} not found.");

		return this.Collections[normalizedKey];
	}
}


