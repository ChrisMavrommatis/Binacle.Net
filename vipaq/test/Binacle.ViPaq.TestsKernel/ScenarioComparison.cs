using Binacle.ViPaq.TestsKernel.Models;

namespace Binacle.ViPaq.TestsKernel;

// The decode-to-input oracle: does a decoded (bin, items) equal the `Scenario` it came from. `Bin`/`Item` are
// plain classes with no value equality, so it compares field by field — which also turns a field-swap wiring bug
// into a clear mismatch. Public and geometry-only so both ViPaq harnesses share it; the paired header-bytes check
// names the internal `Header`, so that half stays with the caller.
public static class ScenarioComparison
{
	public static bool DecodesTo(Scenario scenario, IWithDimensions<ushort> bin, IList<Item<ushort>> items)
	{
		if (!SameDimensions(scenario.Bin, bin) || items.Count != scenario.Items.Length)
		{
			return false;
		}

		for (var index = 0; index < items.Count; index++)
		{
			var expected = scenario.Items[index];
			var actual = items[index];
			if (!SameDimensions(expected, actual) || !SameCoordinates(expected, actual))
			{
				return false;
			}
		}

		return true;
	}

	private static bool SameDimensions(IWithDimensions<ushort> left, IWithDimensions<ushort> right)
		=> left.Length == right.Length && left.Width == right.Width && left.Height == right.Height;

	private static bool SameCoordinates(IWithCoordinates<ushort> left, IWithCoordinates<ushort> right)
		=> left.X == right.X && left.Y == right.Y && left.Z == right.Z;
}
