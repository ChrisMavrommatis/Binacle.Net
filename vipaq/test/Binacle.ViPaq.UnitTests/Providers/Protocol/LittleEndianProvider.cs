using System.Globalization;

namespace Binacle.ViPaq.UnitTests.Providers;

// little-endian/<width>.json as xUnit theory rows: a value paired with its little-endian bytes (low byte
// first). Read tests go bytes -> value, write tests go value -> bytes; both run the same rows. Rows carry
// just the Name (a clean failure label); the test resolves the scenario per width via UInt8(name) /
// UInt16(name). Only the two live wire widths remain (byte / ushort) — the 32/64-bit tiers are gone.
internal static class LittleEndianProvider
{
	public sealed record Scenario<TValue>(TValue Value, byte[] Bytes);

	private static readonly Dictionary<string, Scenario<byte>> uint8;
	private static readonly Dictionary<string, Scenario<ushort>> uint16;

	// A static constructor makes it explicit that the vectors load once, on first access to this provider.
	static LittleEndianProvider()
	{
		uint8 = Load("little-endian/uint8.json", value => (byte)value);
		uint16 = Load("little-endian/uint16.json", value => (ushort)value);
	}

	public static IEnumerable<object[]> UInt8Names => Names(uint8);
	public static IEnumerable<object[]> UInt16Names => Names(uint16);

	public static Scenario<byte> UInt8(string name) => uint8[name];
	public static Scenario<ushort> UInt16(string name) => uint16[name];

	private static IEnumerable<object[]> Names<TValue>(Dictionary<string, Scenario<TValue>> scenarios)
		=> scenarios.Keys.Select(name => new object[] { name });

	private static Dictionary<string, Scenario<TValue>> Load<TValue>(string file, Func<ulong, TValue> cast)
	{
		var scenarios = new Dictionary<string, Scenario<TValue>>();
		foreach (var vector in VectorReader.Read<Vector>(file))
		{
			var scenario = new Scenario<TValue>(cast(ParseValue(vector.Value)), VectorParser.ParseBytes(vector.Bytes));
			scenarios.Add(vector.Name, scenario);
		}
		return scenarios;
	}

	// Value tokens are "0x..." (up to 16 hex digits) — parse to ulong, then Load casts to each width.
	private static ulong ParseValue(string token) =>
		ulong.Parse(token.Replace("_", "")[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture);

	// Raw little-endian/<width>.json row: a value and the little-endian bytes it occupies on the wire.
	private sealed class Vector
	{
		public string Name { get; set; } = "";
		public string Value { get; set; } = "";
		public string[] Bytes { get; set; } = [];
	}
}
