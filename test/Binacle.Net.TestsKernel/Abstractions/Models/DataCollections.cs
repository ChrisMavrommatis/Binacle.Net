using System.Collections.ObjectModel;

namespace Binacle.Net.TestsKernel.Abstractions.Models;

public abstract class DataCollections<T>
	where T : class, new()
{
	private Dictionary<string, T> collections;

	public ReadOnlyDictionary<string, T> Collections => this.collections.AsReadOnly();

	protected DataCollections()
	{
		this.collections = this.InitializeCollections();
	}

	protected abstract Dictionary<string, T> InitializeCollections();

	public virtual T GetCollection(string collectionKey)
	{
		var normalizedKey = collectionKey.ToLower();

		if (!this.collections.ContainsKey(normalizedKey))
			throw new ArgumentException($"Collection with key {normalizedKey} not found.");

		return this.collections[normalizedKey];
	}
}

