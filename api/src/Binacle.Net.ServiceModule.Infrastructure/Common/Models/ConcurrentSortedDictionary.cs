using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Binacle.Net.ServiceModule.Infrastructure.Common.Models;

internal class ConcurrentSortedDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
	where TKey : notnull
{
	private readonly SortedDictionary<TKey, TValue> dict = new();
	private readonly object lockObj = new();

	public int Count
	{
		get
		{
			lock (this.lockObj)
				return this.dict.Count;
		}
	}

	public bool ContainsKey(TKey key)
	{
		lock (this.lockObj)
			return this.dict.ContainsKey(key);
	}

	public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
	{
		lock (this.lockObj)
			return this.dict.TryGetValue(key, out value);
	}

	public TValue this[TKey key]
	{
		get
		{
			lock (this.lockObj)
				return this.dict[key];
		}
		set
		{
			lock (this.lockObj)
				this.dict[key] = value;
		}
	}

	public void Add(TKey key, TValue value)
	{
		lock (this.lockObj)
			this.dict.Add(key, value);
	}

	public bool Remove(TKey key)
	{
		lock (this.lockObj)
			return this.dict.Remove(key);
	}

	public void Clear()
	{
		lock (this.lockObj)
			this.dict.Clear();
	}

	public IEnumerable<TKey> Keys
	{
		get
		{
			lock (this.lockObj)
				return new List<TKey>(this.dict.Keys);
		}
	}

	public IEnumerable<TValue> Values
	{
		get
		{
			lock (this.lockObj)
				return new List<TValue>(this.dict.Values);
		}
	}

	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
	{
		lock (this.lockObj)
			return new List<KeyValuePair<TKey, TValue>>(this.dict).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
