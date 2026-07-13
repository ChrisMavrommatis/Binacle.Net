namespace Binacle.ViPaq.UnitTests.Providers;

// decode-invalid.json as xUnit theory rows. Each scenario is a raw blob the decoder must reject; the
// test only asserts that deserialize throws (the exception type/message differ per language).
internal static class DecodeInvalidProvider
{
	private const string FileName = "serialization/decode-invalid.json";

	public sealed record Scenario(byte[] Blob);

	private static readonly Dictionary<string, Scenario> scenarios;

	// A static constructor makes it explicit that the vectors load once, on first access to this provider.
	static DecodeInvalidProvider()
	{
		scenarios = new Dictionary<string, Scenario>();
		foreach (var vector in VectorReader.Read<Vector>(FileName))
		{
			scenarios.Add(vector.Name, new Scenario(VectorParser.ParseBytes(vector.Blob)));
		}
	}

	public static IEnumerable<object[]> Names
		=> scenarios.Keys.Select(name => new object[] { name });

	public static Scenario Get(string name)
		=> scenarios[name];

	// Raw decode-invalid.json row: a raw blob the decoder must reject (Reason documents why).
	private sealed class Vector
	{
		public string Name { get; set; } = "";
		public string[] Blob { get; set; } = [];
		public string Reason { get; set; } = "";
	}
}
