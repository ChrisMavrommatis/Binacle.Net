using Binacle.Net.TestsKernel.Abstractions;

namespace Binacle.Net.TestsKernel.Models;

public sealed class BinaryDecisionScenarioResult : IScenarioResult
{

	private BinaryDecisionScenarioResult()
	{

	}

	public required bool Fits { get; init; }

	public static IScenarioResult Create(string result)
	{
		bool? fits = null;
		if (result == "Fits")
		{
			fits = true;
		}

		if (result == "DoesNotFit")
		{
			fits = false;
		}

		if (!fits.HasValue)
		{
			throw new InvalidOperationException("Invalid result value for Binary Decision Scenario. Result should be 'Fits' or 'DoesNotFit'");
		}

		return new BinaryDecisionScenarioResult
		{
			Fits = fits!.Value,
		};
	}
}
