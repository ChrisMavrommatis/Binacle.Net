using Binacle.ViPaq.UnitTests.Models;

namespace Binacle.ViPaq.UnitTests.Providers;

// exact-bytes.json as xUnit theory rows. Each row carries just the Name (a clean failure label, since a
// Bin + Item[] + byte[] dump would be unreadable); the test resolves the parsed scenario by name. The
// `Bytes` object is flattened by the blob itself (Blob.ToByteArray).
internal static class ExactBytesProvider
{
	private const string FileName = "exact-bytes.json";

	public sealed record Scenario(Bin<long> Bin, Item<long>[] Items, byte[] Bytes);

	private static readonly Dictionary<string, Scenario> scenarios;

	// A static constructor makes it explicit that the vectors load once, on first access to this provider.
	static ExactBytesProvider()
	{
		scenarios = new Dictionary<string, Scenario>();
		foreach (var vector in VectorReader.Read<Vector>(FileName))
		{
			var bin = VectorParser.ParseBin(vector.Bin);
			var items = VectorParser.ParseItems(vector.Items).ToArray();

			var scenario = new Scenario(
				bin,
				items,
				vector.Bytes.ToByteArray()
			);

			scenarios.Add(vector.Name, scenario);
		}
	}

	public static IEnumerable<object[]> Names
		=> scenarios.Keys.Select(name => new object[] { name });

	public static Scenario Get(string name)
		=> scenarios[name];

	// Raw exact-bytes.json row: a (bin, items) input paired with the exact wire bytes, laid out by segment.
	private sealed class Vector
	{
		public string Name { get; set; } = "";
		public string Bin { get; set; } = "";
		public string[] Items { get; set; } = [];
		public Blob Bytes { get; set; } = new();
	}

	private sealed class Blob
	{
		public string Header { get; set; } = "";
		public string[] Count { get; set; } = [];
		public string[] Bin { get; set; } = [];
		public Item[] Items { get; set; } = [];

		// Flattens the by-segment golden into one wire blob: Header :: Count :: Bin :: (Dims :: Coords per item).
		public byte[] ToByteArray()
		{
			var result = new List<byte> { VectorParser.ParseByte(this.Header) };
			result.AddRange(VectorParser.ParseBytes(this.Count));
			result.AddRange(VectorParser.ParseBytes(this.Bin));
			foreach (var item in this.Items)
			{
				result.AddRange(VectorParser.ParseBytes(item.Dims));
				result.AddRange(VectorParser.ParseBytes(item.Coords));
			}
			return result.ToArray();
		}
	}

	private sealed class Item
	{
		public string[] Dims { get; set; } = [];
		public string[] Coords { get; set; } = [];
	}
}
