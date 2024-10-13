using Binacle.Net.TestsKernel.Abstractions;

namespace Binacle.Net.TestsKernel.Models;

public sealed class PackingEfficiencyScenarioResult : IScenarioResult
{
	private const string format = "{totalItemsVolume} {containerVolume} {totalItemCount}";

	private PackingEfficiencyScenarioResult()
	{
		
	}

	public required long TotalItemsVolume { get; init; }
	public required long ContainerVolume { get; init; }
	public required decimal MaxPotentialPackingEfficiencyPercentage { get; init; }
	public required long TotalItemCount { get; init; }

	public static IScenarioResult Create(string result)
	{
		var resultParts = result.Split(" ");
		if (resultParts.Length != 3)
		{
			throw new InvalidOperationException($"Invalid result format. Format should be {format}");
		}

		var totalItemsVolume = long.Parse(resultParts[0]);
		var containerVolume = long.Parse(resultParts[1]);
		var totalItemCount = long.Parse(resultParts[2]);

		var maxPotentialPackingEfficiencyPercentage = Math.Round(((decimal)totalItemsVolume / (decimal)containerVolume) * 100, 2); 

		return new PackingEfficiencyScenarioResult
		{
			TotalItemsVolume = totalItemsVolume,
			ContainerVolume = containerVolume,
			MaxPotentialPackingEfficiencyPercentage = maxPotentialPackingEfficiencyPercentage,
			TotalItemCount = totalItemCount
		};
	}
}
