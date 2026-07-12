namespace Binacle.ViPaq.UnitTests.Providers;

// header-bytes.json as xUnit theory rows: every Header combo and the two header bytes it packs to. The row
// carries the header notation (both the label and the input); the test resolves the parsed Header and the
// expected bytes by that notation. `Bytes` is an independent golden — the parser that builds the Header never
// sees it, so this is a real check, not a round-trip.
internal static class HeaderBytesProvider
{
	private const string FileName = "header-bytes.json";

	public sealed record Scenario(Header Header, byte[] Bytes);

	// Keyed by the header notation, not a Name field — see Keys below.
	private static readonly Dictionary<string, Scenario> scenarios;

	// A static constructor makes it explicit that the vectors load once, on first access to this provider.
	static HeaderBytesProvider()
	{
		scenarios = new Dictionary<string, Scenario>();
		foreach (var vector in VectorReader.Read<Vector>(FileName))
		{
			var scenario = new Scenario(
				VectorParser.ParseHeader(vector.Header),
				VectorParser.ParseBytes(vector.Bytes)
			);

			scenarios.Add(vector.Header, scenario);
		}
	}

	// Exposes Keys/Get(key), not Names/Get(name) like the other providers: this file has no Name field —
	// the header notation is both the label and the lookup key, so the deviation is intentional.
	public static IEnumerable<object[]> Keys
		=> scenarios.Keys.Select(key => new object[] { key });

	public static Scenario Get(string key)
		=> scenarios[key];

	// Raw header-bytes.json row: the header notation and the two header bytes it packs to.
	private sealed class Vector
	{
		public string Header { get; set; } = "";
		public string[] Bytes { get; set; } = [];
	}
}
