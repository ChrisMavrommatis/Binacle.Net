namespace Binacle.ViPaq.UnitTests.Providers;

// encoding-info-bytes.json as xUnit theory rows: every EncodingInfo combo and the header byte it packs
// to. The row carries the EncodingInfo string (both the label and the input); the test resolves the
// parsed EncodingInfo and expected byte by that string. `Byte` is an independent golden — the parser
// that builds the EncodingInfo never sees it, so this is a real check, not a round-trip.
internal static class EncodingInfoByteProvider
{
	private const string FileName = "encoding-info-bytes.json";

	public sealed record Scenario(EncodingInfo Info, byte Byte);

	// Keyed by the EncodingInfo string, not a Name field — see Keys below.
	private static readonly Dictionary<string, Scenario> scenarios;

	// A static constructor makes it explicit that the vectors load once, on first access to this provider.
	static EncodingInfoByteProvider()
	{
		scenarios = new Dictionary<string, Scenario>();
		foreach (var vector in VectorReader.Read<Vector>(FileName))
		{
			var scenario = new Scenario(
				VectorParser.ParseEncodingInfo(vector.EncodingInfo),
				VectorParser.ParseByte(vector.Byte)
			);

			scenarios.Add(vector.EncodingInfo, scenario);
		}
	}

	// Exposes Keys/Get(key), not Names/Get(name) like the other providers: this file has no Name field —
	// the EncodingInfo string is both the label and the lookup key, so the deviation is intentional.
	public static IEnumerable<object[]> Keys
		=> scenarios.Keys.Select(key => new object[] { key });

	public static Scenario Get(string key)
		=> scenarios[key];

	// Raw encoding-info-bytes.json row: the EncodingInfo string and the header byte it packs to.
	private sealed class Vector
	{
		public string EncodingInfo { get; set; } = "";
		public string Byte { get; set; } = "";
	}
}
