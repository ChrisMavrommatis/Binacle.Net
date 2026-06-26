using System.Numerics;
using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// The reader pulls bytes off the wire little-endian (low byte first) and, for the ReadAs* methods,
// widens the value to T. The plain Read* methods return a fixed width, so T does not matter there;
// the rows below use ProtocolReader<int> for those. The byte vectors are shared with the writer
// tests (see LittleEndianCases), so read and write are checked against the same known bytes.
[Trait("Result Tests", "Ensures results are as expected")]
public class ProtocolReaderTests
{
	// input byte -> same byte out. No endianness to worry about for a single byte.
	[Theory]
	[InlineData(0x00)]
	[InlineData(0xAB)]
	[InlineData(0xFF)]
	public void ReadByte_Reads_The_Byte(byte value)
	{
		var reader = new ProtocolReader<int>(new MemoryStream([value]));

		var result = reader.ReadByte();

		result.ShouldBe(value);
	}

	// bytes in (low byte first) -> the uint16 they spell out.
	[Theory]
	[MemberData(nameof(LittleEndianCases.UInt16), MemberType = typeof(LittleEndianCases))]
	public void ReadUInt16_Reads_Little_Endian(ushort expected, byte[] bytes)
	{
		var reader = new ProtocolReader<int>(new MemoryStream(bytes));

		var result = reader.ReadUInt16();

		result.ShouldBe(expected);
	}

	[Theory]
	[MemberData(nameof(LittleEndianCases.UInt32), MemberType = typeof(LittleEndianCases))]
	public void ReadUInt32_Reads_Little_Endian(uint expected, byte[] bytes)
	{
		var reader = new ProtocolReader<int>(new MemoryStream(bytes));

		var result = reader.ReadUInt32();

		result.ShouldBe(expected);
	}

	[Theory]
	[MemberData(nameof(LittleEndianCases.UInt64), MemberType = typeof(LittleEndianCases))]
	public void ReadUInt64_Reads_Little_Endian(ulong expected, byte[] bytes)
	{
		var reader = new ProtocolReader<int>(new MemoryStream(bytes));

		var result = reader.ReadUInt64();

		result.ShouldBe(expected);
	}

	// The ReadAs* methods read the same little-endian bytes, then widen the value to T. They reuse
	// the same vectors: bytes in, the wider T value out. Without these, the wide readers were only
	// ever exercised through writer round trips.
	[Theory]
	[MemberData(nameof(LittleEndianCases.UInt16), MemberType = typeof(LittleEndianCases))]
	public void ReadAsUInt16_Widens_To_T(ushort wireValue, byte[] bytes)
	{
		var reader = new ProtocolReader<int>(new MemoryStream(bytes));

		var result = reader.ReadAsUInt16();

		result.ShouldBe((int)wireValue);
	}

	[Theory]
	[MemberData(nameof(LittleEndianCases.UInt32), MemberType = typeof(LittleEndianCases))]
	public void ReadAsUInt32_Widens_To_T(uint wireValue, byte[] bytes)
	{
		var reader = new ProtocolReader<long>(new MemoryStream(bytes));

		var result = reader.ReadAsUInt32();

		result.ShouldBe((long)wireValue);
	}

	[Theory]
	[MemberData(nameof(LittleEndianCases.UInt64), MemberType = typeof(LittleEndianCases))]
	public void ReadAsUInt64_Widens_To_T(ulong wireValue, byte[] bytes)
	{
		var reader = new ProtocolReader<ulong>(new MemoryStream(bytes));

		var result = reader.ReadAsUInt64();

		result.ShouldBe(wireValue);
	}

	// A type cannot live inside a data row, so the row carries the Type and this dictionary maps it
	// to the matching generic call. Guards the regression where ReadAsByte only worked when T was int.
	private static readonly Dictionary<Type, Action> ByteReadAssertions = new()
	{
		[typeof(sbyte)] = AssertReadsByteAs<sbyte>,
		[typeof(byte)] = AssertReadsByteAs<byte>,
		[typeof(short)] = AssertReadsByteAs<short>,
		[typeof(ushort)] = AssertReadsByteAs<ushort>,
		[typeof(int)] = AssertReadsByteAs<int>,
		[typeof(uint)] = AssertReadsByteAs<uint>,
		[typeof(long)] = AssertReadsByteAs<long>,
		[typeof(ulong)] = AssertReadsByteAs<ulong>,
	};

	[Theory]
	[InlineData(typeof(sbyte))]
	[InlineData(typeof(byte))]
	[InlineData(typeof(short))]
	[InlineData(typeof(ushort))]
	[InlineData(typeof(int))]
	[InlineData(typeof(uint))]
	[InlineData(typeof(long))]
	[InlineData(typeof(ulong))]
	public void ReadAsByte_Widens_Single_Byte_To_T(Type numericType)
	{
		ByteReadAssertions[numericType]();
	}

	// A single byte must read back as the typed value for any T, not just int.
	private static void AssertReadsByteAs<T>()
		where T : struct, IBinaryInteger<T>, INumber<T>, IComparable<T>
	{
		const byte value = 100; // 0x64, fits every signed and unsigned type
		var reader = new ProtocolReader<T>(new MemoryStream([value]));

		var result = reader.ReadAsByte();

		result.ShouldBe(T.CreateChecked(value));
	}
}
