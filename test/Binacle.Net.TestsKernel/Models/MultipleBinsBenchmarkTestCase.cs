using Binacle.Net.TestsKernel.Models;

namespace Binacle.Net.TestsKernel.Benchmarks.Models;

public sealed class MultipleBinsBenchmarkTestCase
{
	private Dictionary<string, string> binCases;

	public MultipleBinsBenchmarkTestCase()
	{
		this.binCases = new();
	}

	public MultipleBinsBenchmarkTestCase Add(string binString, string result)
	{
		this.binCases.Add(binString, result);
		return this;
	}

	public List<Scenario> GetScenarios(
		ItemsCollection itemsHolder,
		Func<Scenario, bool>? filter = null
	)
	{
		var items = itemsHolder.GetItems();
		var scenarios = new List<Scenario>();
		foreach (var (binString, resultString) in this.binCases)
		{
			var scenario = Scenario.Create($"{binString}::{resultString}", binString, items, resultString);
			if (filter is null || filter(scenario))
			{
				scenarios.Add(scenario);
			}
		}

		return scenarios;
	}
}
