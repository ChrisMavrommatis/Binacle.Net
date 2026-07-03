
namespace Binacle.ViPaq.UnitTests.Providers;

// encode-invalid.json as xUnit theory rows. Each scenario is a (bin, items) input the serializer must
// reject end-to-end; the test only asserts that serialize throws. The `:Q` suffix expands the
// item-count scenario to 65536 items without listing them.
internal static class EncodeInvalidProvider
{
	private const string FileName = "encode-invalid.json";

	public sealed record Scenario(Bin<long> Bin, Item<long>[] Items);

	private static readonly Dictionary<string, Scenario> scenarios;

	// A static constructor makes it explicit that the vectors load once, on first access to this provider.
	static EncodeInvalidProvider()
	{
		scenarios = new Dictionary<string, Scenario>();
		foreach (var vector in VectorReader.Read<Vector>(FileName))
		{
			var scenario = new Scenario(
				VectorParser.ParseBin(vector.Bin),
				VectorParser.ParseItems(vector.Items).ToArray()
			);

			scenarios.Add(vector.Name, scenario);
		}
	}

	public static IEnumerable<object[]> Names
		=> scenarios.Keys.Select(name => new object[] { name });

	public static Scenario Get(string name)
		=> scenarios[name];

	// Raw encode-invalid.json row: a (bin, items) input the serializer must reject (Reason documents why).
	private sealed class Vector
	{
		public string Name { get; set; } = "";
		public string Bin { get; set; } = "";
		public string[] Items { get; set; } = [];
		public string Reason { get; set; } = "";
	}
}
