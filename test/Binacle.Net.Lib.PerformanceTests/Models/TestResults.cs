using System.Collections;

namespace Binacle.Net.Lib.PerformanceTests.Models;


internal class AlgorithmsTestResults<T> : IEnumerable<KeyValuePair<string, List<T>>>
{
	private readonly Dictionary<string, List<T>> dictionary;
	public AlgorithmsTestResults()
	{
		this.dictionary = new Dictionary<string, List<T>>();
	}
	public void AddValue(string algorithmName, T value)
	{
		if (this.dictionary.TryGetValue(algorithmName, out var result))
		{
			result.Add(value);
		}
		else
		{
			this.dictionary.Add(algorithmName, new List<T> { value });
		}
	}
	public IEnumerator<KeyValuePair<string, List<T>>> GetEnumerator()
	{
		return this.dictionary.GetEnumerator();
	}

	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}
}

internal class AlgorithmsDiscrepancies<T> : IEnumerable<KeyValuePair<string, Dictionary<string, Dictionary<string, T>>>>
{
	private readonly Dictionary<string, Dictionary<string, Dictionary<string, T>>> dictionary;

	public AlgorithmsDiscrepancies()
	{
		this.dictionary = new Dictionary<string, Dictionary<string, Dictionary<string, T>>>();
	}

	public void AddDiscrepancy(string algorithmFamilyKey, string scenarioName, Dictionary<string, T> discrepancy)
	{
		this.dictionary.Add(algorithmFamilyKey, new Dictionary<string, Dictionary<string, T>> { { scenarioName, discrepancy } });
	}

	public bool HasDiscrepancies()
	{
		return this.dictionary.Count > 0;
	}

	public int DiscrepancyCategoriesCount()
	{
		return this.dictionary.Keys.Count;
	}

	public int TotalDiscrepancies()
	{
		return this.dictionary.Values.SelectMany(x => x.Values).Count();
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


internal class TestSummaryAction
{
	public string Name { get; set; }
	public Action Action { get; set; }
}
