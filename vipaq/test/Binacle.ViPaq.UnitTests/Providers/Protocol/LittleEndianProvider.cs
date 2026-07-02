using System.Globalization;

namespace Binacle.ViPaq.UnitTests.Providers;

// little-endian/<width>.json as xUnit theory rows: a value paired with its little-endian bytes (low byte
// first). Read tests go bytes -> value, write tests go value -> bytes; both run the same rows. Rows carry
// just the Name (a clean failure label); the test resolves the scenario per width via UInt8(name) ..
// UInt64(name). There is one dictionary per width because each width's tests want Value already typed to
// that width (byte / ushort / uint / ulong).
internal static class LittleEndianProvider
{
	public sealed record Scenario<TValue>(TValue Value, byte[] Bytes);

	private static readonly Dictionary<string, Scenario<byte>> uint8;
	private static readonly Dictionary<string, Scenario<ushort>> uint16;
	private static readonly Dictionary<string, Scenario<uint>> uint32;
	private static readonly Dictionary<string, Scenario<ulong>> uint64;

	// A static constructor makes it explicit that the vectors load once, on first access to this provider.
	static LittleEndianProvider()
	{
		uint8 = Load("little-endian.uint8.json", value => (byte)value);
		uint16 = Load("little-endian.uint16.json", value => (ushort)value);
		uint32 = Load("little-endian.uint32.json", value => (uint)value);
		uint64 = Load("little-endian.uint64.json", value => value);

		// The shared files stop at the interoperable ceiling (2^53-1) so the values stay exact in JS. These
		// two rows are above that ceiling, so they are deliberately C#-local (not in the shared files, not
		// in TS): a fully distinct byte pattern pins all 8 little-endian positions, and all-bits pins
		// saturation. Read64Bits / Write64Bits are raw primitives with no interoperable ceiling, so the
		// wide values are valid here (the ceiling is enforced by the pickers and the decode path, not here).
		uint64.Add(
			"all bytes distinct 0x0102030405060708 -> 08 07 06 05 04 03 02 01 (C#-local, above interoperable range)",
			new Scenario<ulong>(0x0102030405060708UL, [0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01]));
		uint64.Add(
			"all bits set 0xFFFFFFFFFFFFFFFF -> FF x8 (C#-local, above interoperable range)",
			new Scenario<ulong>(0xFFFFFFFFFFFFFFFFUL, [0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF]));
	}

	public static IEnumerable<object[]> UInt8Names => Names(uint8);
	public static IEnumerable<object[]> UInt16Names => Names(uint16);
	public static IEnumerable<object[]> UInt32Names => Names(uint32);
	public static IEnumerable<object[]> UInt64Names => Names(uint64);

	public static Scenario<byte> UInt8(string name) => uint8[name];
	public static Scenario<ushort> UInt16(string name) => uint16[name];
	public static Scenario<uint> UInt32(string name) => uint32[name];
	public static Scenario<ulong> UInt64(string name) => uint64[name];

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
