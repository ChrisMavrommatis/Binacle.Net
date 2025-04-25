using System.Collections;

namespace Binacle.Net;

public class ConcurrentSortedDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
	where TKey : notnull
{
	private readonly SortedDictionary<TKey, TValue> _dict = new();
	private readonly object _lock = new();

	public int Count
	{
		get
		{
			lock (_lock)
				return _dict.Count;
		}
	}

	public bool ContainsKey(TKey key)
	{
		lock (_lock)
			return _dict.ContainsKey(key);
	}

	public bool TryGetValue(TKey key, out TValue value)
	{
		lock (_lock)
			return _dict.TryGetValue(key, out value);
	}

	public TValue this[TKey key]
	{
		get
		{
			lock (_lock)
				return _dict[key];
		}
		set
		{
			lock (_lock)
				_dict[key] = value;
		}
	}

	public void Add(TKey key, TValue value)
	{
		lock (_lock)
			_dict.Add(key, value);
	}

	public bool Remove(TKey key)
	{
		lock (_lock)
			return _dict.Remove(key);
	}

	public void Clear()
	{
		lock (_lock)
			_dict.Clear();
	}

	public IEnumerable<TKey> Keys
	{
		get
		{
			lock (_lock)
				return new List<TKey>(_dict.Keys);
		}
	}

	public IEnumerable<TValue> Values
	{
		get
		{
			lock (_lock)
				return new List<TValue>(_dict.Values);
		}
	}

	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
	{
		lock (_lock)
			return new List<KeyValuePair<TKey, TValue>>(_dict).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
