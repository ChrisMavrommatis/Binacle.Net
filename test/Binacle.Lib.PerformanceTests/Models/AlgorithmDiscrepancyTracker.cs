using System.Collections;
using System.Numerics;

namespace Binacle.Lib.PerformanceTests.Models;

internal class AlgorithmDiscrepancyTracker<T> : IEnumerable<KeyValuePair<string, Dictionary<string, Dictionary<string, T>>>>
	where T : struct,  INumber<T>, IComparable<T>
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

	public List<DiscrepancyResults<T>> GetDiscrepancyResults(string unit)
	{
		var list = new List<DiscrepancyResults<T>			//
>();
		foreach(var (familyKey, scenarioDiscrepancies) in this.dictionary)
		{
			var discrepancyResults = new DiscrepancyResults<T>()
			{
				Unit = unit,
				Family = familyKey,
				Algorithms = scenarioDiscrepancies.SelectMany(x => x.Value.Keys).Distinct().ToArray(),
				ScenarioDiscrepancies = scenarioDiscrepancies
			};
			// Scenario, Alg 1, alg 2
			list.Add(discrepancyResults);
		}
		return list;
	}
}

