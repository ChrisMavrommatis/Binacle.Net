using Binacle.TestsKernel.Algorithms.Helpers;
using Binacle.TestsKernel.Models;

namespace Binacle.TestsKernel.Algorithms.Models;

public record CollectionScenario(
	string ConnectionKey,
	Scenario Scenario
);


public class Scenario
{
	public required string Name { get; init; }
	public required TestBin Bin { get; init; }
	public required ScenarioMetrics Metrics { get; init; }
	public required List<TestItem> Items { get; init; }
	
	public required ScenarioResult Result { get; init; }
	public override string ToString() => Name;


	public static Scenario Create(
		string name, 
		string bin,
		string[] items,
		string metrics,
		string result)
	{
		var parsedMetrics = ScenarioMetricsHelper.ParseFromCompactString(metrics);
		var parsedResult = ScenarioResultHelper.ParseFromCompactString(result);
		return new Scenario
		{
			Name = name,
			Bin = TestBin.FromCompactString(bin),
			Metrics = parsedMetrics,
			Items = items.Select(TestItem.FromCompactString).ToList(),
			Result = parsedResult
		};
	}
}
