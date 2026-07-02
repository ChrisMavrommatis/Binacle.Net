using Binacle.ViPaq.UnitTests.Models;

namespace Binacle.ViPaq.UnitTests.Providers;

// round-trip-scenarios.json as xUnit theory rows. Each scenario is a (bin, items) input plus the header
// the serializer must produce (ExpectedEncodingInfo). The test serializes, checks byte 0 == that
// header, then deserializes and asserts the items come back unchanged.
internal static class RoundTripProvider
{
	private const string FileName = "round-trip-scenarios.json";

	public sealed record Scenario(Bin<long> Bin, Item<long>[] Items, EncodingInfo ExpectedEncodingInfo);

	private static readonly Dictionary<string, Scenario> scenarios;

	// A static constructor makes it explicit that the vectors load once, on first access to this provider.
	static RoundTripProvider()
	{
		scenarios = new Dictionary<string, Scenario>();
		foreach (var vector in VectorReader.Read<Vector>(FileName))
		{
			var scenario = new Scenario(
				VectorParser.ParseBin(vector.Bin),
				VectorParser.ParseItems(vector.Items).ToArray(),
				VectorParser.ParseEncodingInfo(vector.ExpectedEncodingInfo)
			);

			scenarios.Add(vector.Name, scenario);
		}
	}

	public static IEnumerable<object[]> Names
		=> scenarios.Keys.Select(name => new object[] { name });

	public static Scenario Get(string name)
		=> scenarios[name];

	// Raw round-trip-scenarios.json row: a (bin, items) input plus the header the serializer must produce.
	private sealed class Vector
	{
		public string Name { get; set; } = "";
		public string Bin { get; set; } = "";
		public string[] Items { get; set; } = [];
		public string ExpectedEncodingInfo { get; set; } = "";
	}
}
