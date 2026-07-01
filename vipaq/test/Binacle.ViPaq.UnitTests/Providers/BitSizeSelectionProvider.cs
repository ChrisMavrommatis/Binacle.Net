using Binacle.ViPaq.UnitTests.Models;

namespace Binacle.ViPaq.UnitTests.Providers;

// bit-size-selection.json as xUnit theory rows. Width selection is identical for dimensions and
// coordinates, so each scenario is parsed into both a Dimensions and a Coordinates triple and the test
// runs both pickers — both must return the same expected width.
internal static class BitSizeSelectionProvider
{
	private const string FileName = "bit-size-selection.json";

	public sealed record Scenario(Dimensions<long> Dimensions, Coordinates<long> Coordinates, BitSize Expected);

	private static readonly Dictionary<string, Scenario> scenarios;

	// A static constructor makes it explicit that the vectors load once, on first access to this provider.
	static BitSizeSelectionProvider()
	{
		scenarios = new Dictionary<string, Scenario>();
		foreach (var vector in VectorReader.Read<Vector>(FileName))
		{
			// The one Values triple is parsed as both a Dimensions and a Coordinates so the test can run
			// it through both pickers — this scenario checks they agree, so both triples are exercised.
			var scenario = new Scenario(
				VectorParser.ParseDimensions(vector.Values),
				VectorParser.ParseCoordinates(vector.Values),
				vector.ExpectedBitSize
			);

			scenarios.Add(vector.Name, scenario);
		}
	}

	public static IEnumerable<object[]> Names
		=> scenarios.Keys.Select(name => new object[] { name });

	public static Scenario Get(string name)
		=> scenarios[name];

	// Raw bit-size-selection.json row: a values triple and the width the picker must choose for it.
	private sealed class Vector
	{
		public string Name { get; set; } = "";
		public string Values { get; set; } = "";
		public BitSize ExpectedBitSize { get; set; }
	}
}
