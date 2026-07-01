using Binacle.ViPaq.UnitTests.Models;

namespace Binacle.ViPaq.UnitTests.Providers;

// bit-size-invalid.json as xUnit theory rows. Each scenario is a values triple the picker must reject.
// Kind routes to the right picker (dimensions reject 0, coordinates allow 0); Field is the offending
// field's PascalCase name, which equals the thrown ArgumentException ParamName.
internal static class BitSizeInvalidProvider
{
	private const string FileName = "bit-size-invalid.json";

	// Which picker a row targets. Public because Scenario exposes it and BitSizeInvalidTests routes on it.
	public enum BitSizeKind
	{
		Dimensions,
		Coordinates,
	}

	public sealed record Scenario(BitSizeKind Kind, Dimensions<long> Dimensions, Coordinates<long> Coordinates, string Field);

	private static readonly Dictionary<string, Scenario> scenarios;

	// A static constructor makes it explicit that the vectors load once, on first access to this provider.
	static BitSizeInvalidProvider()
	{
		scenarios = new Dictionary<string, Scenario>();
		foreach (var vector in VectorReader.Read<Vector>(FileName))
		{
			// The values triple is parsed as both a Dimensions and a Coordinates, but only the Kind-matching
			// one is exercised; the other is intentionally inert, kept so the shape matches the selection rows.
			var scenario = new Scenario(
				vector.Kind,
				VectorParser.ParseDimensions(vector.Values),
				VectorParser.ParseCoordinates(vector.Values),
				vector.Field
			);

			scenarios.Add(vector.Name, scenario);
		}
	}

	public static IEnumerable<object[]> Names
		=> scenarios.Keys.Select(name => new object[] { name });

	public static Scenario Get(string name)
		=> scenarios[name];

	// Raw bit-size-invalid.json row: a values triple the picker must reject, plus which picker and field.
	private sealed class Vector
	{
		public string Name { get; set; } = "";
		public BitSizeKind Kind { get; set; }    // routes to the right picker; bound from the JSON string
		public string Values { get; set; } = "";
		public string Field { get; set; } = ""; // PascalCase field name = the thrown ArgumentException ParamName
	}
}
