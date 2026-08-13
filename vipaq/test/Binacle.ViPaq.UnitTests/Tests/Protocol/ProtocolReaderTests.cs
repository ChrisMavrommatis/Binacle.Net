using System.Numerics;
using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// The reader pulls bytes off the wire little-endian. ReadUInt16 returns a fixed width and is used for the
// header, so T does not matter there. Read8Bits / Read16Bits read the same bytes but widen the value to T. The
// byte vectors are the shared little-endian/<width>.json files, so read and write grade against the same
// bytes.
[Trait("Result Tests", "Ensures results are as expected")]
public class ProtocolReaderTests
{
	// input byte -> same byte out, widened to T. No endianness to worry about for a single byte.
	[Theory]
	[MemberData(nameof(LittleEndianProvider.UInt8Names), MemberType = typeof(LittleEndianProvider))]
	public void Read8Bits_Reads_The_Byte(string name)
	{
		var (expected, bytes) = LittleEndianProvider.UInt8(name);
		var reader = new ProtocolReader<int>(new MemoryStream(bytes));

		var result = reader.Read8Bits();

		result.ShouldBe((int)expected);
	}

	// At end of stream a single-byte read must throw, not hand back a phantom byte, or a truncated 8-bit body
	// would be accepted.
	[Fact]
	public void Read8Bits_Throws_At_End_Of_Stream()
	{
		var reader = new ProtocolReader<int>(new MemoryStream([]));

		Should.Throw<EndOfStreamException>(() => reader.Read8Bits());
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

	// Read16Bits reads the same little-endian bytes, then widens to T. The only place the 16-bit byte order is
	// pinned on the read side.
	[Theory]
	[MemberData(nameof(LittleEndianProvider.UInt16Names), MemberType = typeof(LittleEndianProvider))]
	public void Read16Bits_Widens_To_T(string name)
	{
		var (wireValue, bytes) = LittleEndianProvider.UInt16(name);
		var reader = new ProtocolReader<int>(new MemoryStream(bytes));

		var result = reader.Read16Bits();

		result.ShouldBe((int)wireValue);
	}

	private const byte SingleByte = 100; // 0x64, fits every signed and unsigned type

	// A type cannot live inside a data row, so the row carries the Type and these maps reach the matching
	// generic call. Two maps rather than one returning both halves, so arrange, act and assert stay on three
	// lines. Guards the regression where Read8Bits only worked when T was int.
	private static readonly Dictionary<Type, Func<object>> ExpectedSingleByteByType = new()
	{
		[typeof(sbyte)] = ExpectedSingleByteAs<sbyte>,
		[typeof(byte)] = ExpectedSingleByteAs<byte>,
		[typeof(short)] = ExpectedSingleByteAs<short>,
		[typeof(ushort)] = ExpectedSingleByteAs<ushort>,
		[typeof(int)] = ExpectedSingleByteAs<int>,
		[typeof(uint)] = ExpectedSingleByteAs<uint>,
		[typeof(long)] = ExpectedSingleByteAs<long>,
		[typeof(ulong)] = ExpectedSingleByteAs<ulong>,
	};

	private static readonly Dictionary<Type, Func<object>> SingleByteReadsByType = new()
	{
		[typeof(sbyte)] = ReadSingleByteAs<sbyte>,
		[typeof(byte)] = ReadSingleByteAs<byte>,
		[typeof(short)] = ReadSingleByteAs<short>,
		[typeof(ushort)] = ReadSingleByteAs<ushort>,
		[typeof(int)] = ReadSingleByteAs<int>,
		[typeof(uint)] = ReadSingleByteAs<uint>,
		[typeof(long)] = ReadSingleByteAs<long>,
		[typeof(ulong)] = ReadSingleByteAs<ulong>,
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
		var expected = ExpectedSingleByteByType[numericType]();

		var actual = SingleByteReadsByType[numericType]();

		actual.ShouldBe(expected);
	}

	// The literal widened to T: what a single-byte read should hand back.
	private static object ExpectedSingleByteAs<T>()
		where T : struct, IBinaryInteger<T>
		=> T.CreateChecked(SingleByte);

	// Boxed so the row can carry it. Both sides box the same T, so equality compares like with like.
	private static object ReadSingleByteAs<T>()
		where T : struct, IBinaryInteger<T>
	{
		var stream = new MemoryStream([SingleByte]);
		var reader = new ProtocolReader<T>(stream);

		return reader.Read8Bits();
	}
}
