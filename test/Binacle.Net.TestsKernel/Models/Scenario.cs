using Binacle.Net.TestsKernel.Abstractions;

namespace Binacle.Net.TestsKernel.Models;

public sealed class Scenario : BinScenarioBase
{
	private Scenario(string binString) : base(binString)
	{

	}

	public required IScenarioResult Result { get; init; }
	public required ScenarioResultType ResultType { get; init; }
	public required string Name { get; init; }
	public required List<TestItem> Items { get; init; }

	public TResult ResultAs<TResult>()
		where TResult : class, IScenarioResult
	{
		if (this.Result is TResult result)
		{
			return result;
		}

		throw new InvalidOperationException($"Result is not of type {typeof(TResult).Name}");
	}

	internal static Scenario Create(string name, string bin, List<TestItem> items, string result)
	{
		var resultParts = result.Split("::");
		if(resultParts.Length != 2)
		{
			throw new ArgumentException("Invalid result format. Format should be {ResultType::Result}");
		}

		if(!Enum.TryParse<ScenarioResultType>(resultParts[0], out var resultType))
		{
			throw new ArgumentException("Invalid result type");
		}

		var scenarioResult = resultType switch
		{
			ScenarioResultType.BinaryDecision => BinaryDecisionScenarioResult.Create(resultParts[1]),
			ScenarioResultType.PackingEfficiency => PackingEfficiencyScenarioResult.Create(resultParts[1]),
			_ => throw new NotImplementedException($"Result type {resultParts[0]} not implemented")
		};
		return new Scenario(bin)
		{
			Name = name,
			Items = items,
			Result = scenarioResult,
			ResultType = resultType
		};
	}
}
