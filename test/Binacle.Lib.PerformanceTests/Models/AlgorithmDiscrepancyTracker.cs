using System.Collections;

namespace Binacle.Lib.PerformanceTests.Models;

internal class AlgorithmDiscrepancyTracker<T> : IEnumerable<KeyValuePair<string, Dictionary<string, Dictionary<string, T>>>>
{
	private readonly Dictionary<string, Dictionary<string, Dictionary<string, T>>> dictionary;

	public AlgorithmDiscrepancyTracker()
	{
		this.dictionary = new Dictionary<string, Dictionary<string, Dictionary<string, T>>>();
	}

	public void AddDiscrepancy(string algorithmFamilyKey, string scenarioName, Dictionary<string, T> discrepancy)
	{
		if (!this.dictionary.TryGetValue(algorithmFamilyKey, out var family))
		{
			family = new Dictionary<string, Dictionary<string, T>>();
			this.dictionary[algorithmFamilyKey] = family;
		}
		family[scenarioName] = discrepancy;
	}

	public bool HasDiscrepancies()
	{
		return this.dictionary.Count > 0;
	}

	public int DiscrepancyCategoriesCount()
	{
		return this.dictionary.Count;
	}

	public int TotalDiscrepancies()
	{
		return this.dictionary.Values
			.SelectMany(family => family.Values)
			.Sum(discrepancy => discrepancy.Count);
	}

	public IEnumerator<KeyValuePair<string, Dictionary<string, Dictionary<string, T>>>> GetEnumerator()
	{
		return this.dictionary.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}
}
