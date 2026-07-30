
namespace Binacle.ViPaq.UnitTests.Providers;

// width-selection.json as xUnit theory rows. Kind splits the rows into dimensions ("LxWxH") and
// coordinates ("X,Y,Z"); each row runs through its own picker and must return the expected width. Both pickers
// use identical width math, and the two sets together cover every width bucket, so they can't drift apart.
internal static class WidthSelectionProvider
{
	private const string FileName = "width/width-selection.json";

	public sealed record Scenario<TValue>(TValue Value, Width Expected);

	private static readonly Dictionary<string, Scenario<Dimensions<long>>> dimensions;
	private static readonly Dictionary<string, Scenario<Coordinates<long>>> coordinates;

	// A static constructor makes it explicit that the vectors load once, on first access to this provider.
	static WidthSelectionProvider()
	{
		dimensions = new Dictionary<string, Scenario<Dimensions<long>>>();
		coordinates = new Dictionary<string, Scenario<Coordinates<long>>>();
		foreach (var vector in VectorReader.Read<Vector>(FileName))
		{
			// Values is parsed by Kind, and the row lands in that kind's set only.
			if (vector.Kind == WidthKind.Dimensions)
				dimensions.Add(vector.Name, new Scenario<Dimensions<long>>(VectorParser.ParseDimensions(vector.Values), vector.ExpectedWidth));
			else
				coordinates.Add(vector.Name, new Scenario<Coordinates<long>>(VectorParser.ParseCoordinates(vector.Values), vector.ExpectedWidth));
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

	// Raw width-selection.json row: a values triple (format per Kind) and the width the picker must choose.
	private sealed class Vector
	{
		public string Name { get; set; } = "";
		public WidthKind Kind { get; set; }    // splits the rows and says how Values is parsed
		public string Values { get; set; } = "";
		public Width ExpectedWidth { get; set; }
	}
}
