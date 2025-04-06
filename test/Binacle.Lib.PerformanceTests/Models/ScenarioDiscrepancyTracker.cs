using System.Collections;
using System.Numerics;

namespace Binacle.Lib.PerformanceTests.Models;

internal class ScenarioDiscrepancyTracker<T> : IEnumerable<KeyValuePair<string, Dictionary<string, T>>>
	where T : struct, INumber<T>, IComparable<T>
{
	private readonly Dictionary<string, Dictionary<string, T>> dictionary;

	public ScenarioDiscrepancyTracker()
	{
		this.dictionary = new Dictionary<string, Dictionary<string, T>>();
	}

	public void AddValue(string scenarioName, string algorithm, T value)
	{
		if (!this.dictionary.TryGetValue(scenarioName, out var discrepancy))
		{
			discrepancy = new Dictionary<string, T>();
			this.dictionary[scenarioName] = discrepancy;
		}
		discrepancy[algorithm] = value;
	}

	public ComparisonResults<T> GetDiscrepanciesComparisonResults(string unit, string baselineAlgorithm)
	{
		var comparisonResults = new ComparisonResults<T>
		{
			Unit = unit
		};
		foreach (var (scenarioName, scenarioValues) in this)
		{
			if(this.HasDiscrepancies(scenarioName, baselineAlgorithm, scenarioValues))
			{
				foreach (var (algorithm, value) in scenarioValues)
				{
					comparisonResults.Add(scenarioName, algorithm, value);
				}
			}
		}
		return comparisonResults;
	}

	private bool HasDiscrepancies(string scenarioName, string baselineAlgorithm, Dictionary<string, T> scenarioValues)
	{
		if (!scenarioValues.TryGetValue(baselineAlgorithm, out var baselineValue))
		{
			throw new InvalidOperationException($"Baseline value for scenario '{scenarioName}' not found.");
		}
		foreach (var (algorithm, value) in scenarioValues)
		{
			if (algorithm == baselineAlgorithm)
			{
				continue;
			} 
			if(baselineValue < value)
			{
				return true;
			}
		}
		return false;
	}

	public IEnumerator<KeyValuePair<string, Dictionary<string, T>>> GetEnumerator()
	{
		return this.dictionary.GetEnumerator();
	}
	
	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}
}
