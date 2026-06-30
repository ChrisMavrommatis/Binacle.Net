using System.Numerics;
using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// The reader pulls bytes off the wire little-endian (low byte first). ReadByte / ReadUInt16 return a
// fixed width and are used for the header, so T does not matter there; the rows below use
// ProtocolReader<int> for those. Read8Bits..Read64Bits read the same bytes but widen the value to T.
// The byte vectors are shared with the writer tests (see LittleEndianCases), so read and write are
// checked against the same known bytes.
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

	// Read8Bits..Read64Bits read the same little-endian bytes, then widen the value to T. They reuse
	// the same vectors: bytes in, the wider T value out. These are the only place the 32- and 64-bit
	// little-endian order is pinned now that the unused concrete readers are gone.
	[Theory]
	[MemberData(nameof(LittleEndianCases.UInt16), MemberType = typeof(LittleEndianCases))]
	public void Read16Bits_Widens_To_T(ushort wireValue, byte[] bytes)
	{
		var reader = new ProtocolReader<int>(new MemoryStream(bytes));

		var result = reader.Read16Bits();

		result.ShouldBe((int)wireValue);
	}

	[Theory]
	[MemberData(nameof(LittleEndianCases.UInt32), MemberType = typeof(LittleEndianCases))]
	public void Read32Bits_Widens_To_T(uint wireValue, byte[] bytes)
	{
		var reader = new ProtocolReader<long>(new MemoryStream(bytes));

		var result = reader.Read32Bits();

		result.ShouldBe((long)wireValue);
	}

	[Theory]
	[MemberData(nameof(LittleEndianCases.UInt64), MemberType = typeof(LittleEndianCases))]
	public void Read64Bits_Widens_To_T(ulong wireValue, byte[] bytes)
	{
		var reader = new ProtocolReader<ulong>(new MemoryStream(bytes));

		var result = reader.Read64Bits();

		result.ShouldBe(wireValue);
	}

	// A type cannot live inside a data row, so the row carries the Type and this dictionary maps it
	// to the matching generic call. Guards the regression where Read8Bits only worked when T was int.
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
	public void Read8Bits_Widens_Single_Byte_To_T(Type numericType)
	{
		ByteReadAssertions[numericType]();
	}

	// A single byte must read back as the typed value for any T, not just int.
	private static void AssertReadsByteAs<T>()
		where T : struct, IBinaryInteger<T>, INumber<T>, IComparable<T>
	{
		const byte value = 100; // 0x64, fits every signed and unsigned type
		var reader = new ProtocolReader<T>(new MemoryStream([value]));

		var result = reader.Read8Bits();

		result.ShouldBe(T.CreateChecked(value));
	}
}
