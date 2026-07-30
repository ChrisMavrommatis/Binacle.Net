using Binacle.ViPaq.TestsKernel;
using Binacle.ViPaq.TestsKernel.Models;

namespace Binacle.ViPaq.PerformanceTests.PreReportChecks;

// A token round-trips only if its two header bytes decode back to the header written (a `Header.FromBytes` bug
// otherwise hides behind a geometry-only assert) and the pack decodes back to input. Compressed bytes are never
// compared — the oracle is always decode-to-input (PROTOCOL.md §6.1).
internal static class RoundTripAssertion
{
	public static void Assert(
		Scenario scenario, 
		byte[] token, 
		Header expectedHeader,
		IWithDimensions<ushort> bin,
		IList<Item<ushort>> items, 
		string mode
	)
	{
		if (Header.FromBytes(token[0], token[1]) != expectedHeader)
		{
			throw new InvalidOperationException($"Scenario '{scenario.Name}' header did not round-trip ({mode}).");
		}

		if (!ScenarioComparison.DecodesTo(scenario, bin, items))
		{
			throw new InvalidOperationException($"Scenario '{scenario.Name}' did not round-trip ({mode}).");
		}
	}
}
