using Binacle.ViPaq.UnitTests.Models;

namespace Binacle.ViPaq.UnitTests.Providers;

// bit-size-invalid.json as xUnit theory rows. Kind splits the rows into two sets, each a values triple its
// picker must reject: dimensions ("LxWxH") and coordinates ("X,Y,Z"). Field is the offending field's
// PascalCase name, which equals the thrown ArgumentException ParamName. A row is one kind, never both.
internal static class BitSizeInvalidProvider
{
	private const string FileName = "bit-size-invalid.json";

	public sealed record Scenario<TValue>(TValue Value, string Field);

	private static readonly Dictionary<string, Scenario<Dimensions<long>>> dimensions;
	private static readonly Dictionary<string, Scenario<Coordinates<long>>> coordinates;

	// A static constructor makes it explicit that the vectors load once, on first access to this provider.
	static BitSizeInvalidProvider()
	{
		dimensions = new Dictionary<string, Scenario<Dimensions<long>>>();
		coordinates = new Dictionary<string, Scenario<Coordinates<long>>>();
		foreach (var vector in VectorReader.Read<Vector>(FileName))
		{
			// Values is parsed by Kind, and the row lands in that kind's set only.
			if (vector.Kind == BitSizeKind.Dimensions)
				dimensions.Add(vector.Name, new Scenario<Dimensions<long>>(VectorParser.ParseDimensions(vector.Values), vector.Field));
			else
				coordinates.Add(vector.Name, new Scenario<Coordinates<long>>(VectorParser.ParseCoordinates(vector.Values), vector.Field));
		}
	}

	public static IEnumerable<object[]> DimensionNames
		=> dimensions.Keys.Select(name => new object[] { name });

	public static IEnumerable<object[]> CoordinateNames
		=> coordinates.Keys.Select(name => new object[] { name });

	public static Scenario<Dimensions<long>> Dimension(string name)
		=> dimensions[name];

	public static Scenario<Coordinates<long>> Coordinate(string name)
		=> coordinates[name];

	// Raw bit-size-invalid.json row: a values triple the picker must reject, plus which picker and field.
	private sealed class Vector
	{
		public string Name { get; set; } = "";
		public BitSizeKind Kind { get; set; }    // splits the rows and says how Values is parsed
		public string Values { get; set; } = "";
		public string Field { get; set; } = ""; // PascalCase field name = the thrown ArgumentException ParamName
	}
}
