namespace Binacle.Net.TestsKernel.Models;

public sealed class MultiBinsBenchmarkTestCase
{
	private Dictionary<string, string> binCases;

	public MultiBinsBenchmarkTestCase()
	{
		this.binCases = new();
	}

	public MultiBinsBenchmarkTestCase Add(string binString, string result)
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
