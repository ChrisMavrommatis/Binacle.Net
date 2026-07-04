using System.Numerics;
using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// The reader pulls bytes off the wire little-endian (low byte first). ReadByte / ReadUInt16 return a
// fixed width and are used for the header, so T does not matter there; the rows below use
// ProtocolReader<int> for those. Read8Bits..Read64Bits read the same bytes but widen the value to T.
// The byte vectors are the shared little-endian/<width>.json files, also used by the writer tests, so
// read and write are checked against the same known bytes.
[Trait("Result Tests", "Ensures results are as expected")]
public class ProtocolReaderTests
{
	// input byte -> same byte out. No endianness to worry about for a single byte.
	[Theory]
	[MemberData(nameof(LittleEndianProvider.UInt8Names), MemberType = typeof(LittleEndianProvider))]
	public void ReadByte_Reads_The_Byte(string name)
	{
		var (expected, bytes) = LittleEndianProvider.UInt8(name);
		var reader = new ProtocolReader<int>(new MemoryStream(bytes));

		var result = reader.ReadByte();

		result.ShouldBe(expected);
	}

	// At end of stream a single-byte read must throw, not hand back a phantom byte. This is the lib-level
	// pin for the fix that lets a truncated 8-bit body be rejected (see the shared decode-invalid vector).
	[Fact]
	public void ReadByte_Throws_At_End_Of_Stream()
	{
		var reader = new ProtocolReader<int>(new MemoryStream([]));

		Should.Throw<EndOfStreamException>(() => reader.ReadByte());
	}

	// bytes in (low byte first) -> the uint16 they spell out.
	[Theory]
	[MemberData(nameof(LittleEndianProvider.UInt16Names), MemberType = typeof(LittleEndianProvider))]
	public void ReadUInt16_Reads_Little_Endian(string name)
	{
		var (expected, bytes) = LittleEndianProvider.UInt16(name);
		var reader = new ProtocolReader<int>(new MemoryStream(bytes));

		var result = reader.ReadUInt16();

		result.ShouldBe(expected);
	}

	// Read8Bits..Read64Bits read the same little-endian bytes, then widen the value to T. They reuse
	// the same vectors: bytes in, the wider T value out. These are the only place the 32- and 64-bit
	// little-endian order is pinned now that the unused concrete readers are gone.
	[Theory]
	[MemberData(nameof(LittleEndianProvider.UInt16Names), MemberType = typeof(LittleEndianProvider))]
	public void Read16Bits_Widens_To_T(string name)
	{
		var (wireValue, bytes) = LittleEndianProvider.UInt16(name);
		var reader = new ProtocolReader<int>(new MemoryStream(bytes));

		var result = reader.Read16Bits();

		result.ShouldBe((int)wireValue);
	}

	[Theory]
	[MemberData(nameof(LittleEndianProvider.UInt32Names), MemberType = typeof(LittleEndianProvider))]
	public void Read32Bits_Widens_To_T(string name)
	{
		var (wireValue, bytes) = LittleEndianProvider.UInt32(name);
		var reader = new ProtocolReader<long>(new MemoryStream(bytes));

		var result = reader.Read32Bits();

		result.ShouldBe((long)wireValue);
	}

	[Theory]
	[MemberData(nameof(LittleEndianProvider.UInt64Names), MemberType = typeof(LittleEndianProvider))]
	public void Read64Bits_Widens_To_T(string name)
	{
		var (wireValue, bytes) = LittleEndianProvider.UInt64(name);
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
		where T : struct, IBinaryInteger<T>
	{
		const byte value = 100; // 0x64, fits every signed and unsigned type
		var reader = new ProtocolReader<T>(new MemoryStream([value]));

		var result = reader.Read8Bits();

		result.ShouldBe(T.CreateChecked(value));
	}
}
