using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// The writer puts bytes on the wire little-endian (low byte first). WriteByte / WriteUInt16 take a
// fixed width and are used for the header. Write8Bits..Write64Bits narrow T down to the wire width
// first, then write the same bytes. These pin the exact byte order, and they reuse the same vectors
// as the reader tests (see LittleEndianCases) so both sides agree on the bytes.
[Trait("Result Tests", "Ensures results are as expected")]
public class ProtocolWriterTests
{
	// input byte -> same byte out. No endianness to worry about for a single byte.
	[Theory]
	[InlineData(0x00)]
	[InlineData(0xAB)]
	[InlineData(0xFF)]
	public void WriteByte_Writes_The_Byte(byte value)
	{
		using var stream = new MemoryStream();
		var writer = new ProtocolWriter<int>(stream);

		writer.WriteByte(value);

		stream.ToArray().ShouldBe(new[] { value });
	}

	// value in -> the little-endian bytes it occupies on the wire (low byte first).
	[Theory]
	[MemberData(nameof(LittleEndianCases.UInt16), MemberType = typeof(LittleEndianCases))]
	public void WriteUInt16_Writes_Little_Endian(ushort value, byte[] expected)
	{
		using var stream = new MemoryStream();
		var writer = new ProtocolWriter<int>(stream);

		writer.WriteUInt16(value);

		stream.ToArray().ShouldBe(expected);
	}

	// input byte -> same byte out, narrowed from T first.
	[Theory]
	[InlineData(0x00)]
	[InlineData(0xAB)]
	[InlineData(0xFF)]
	public void Write8Bits_Narrows_T_And_Writes(byte value)
	{
		using var stream = new MemoryStream();
		var writer = new ProtocolWriter<int>(stream);

		writer.Write8Bits(value);

		stream.ToArray().ShouldBe(new[] { value });
	}

	// Write16Bits..Write64Bits narrow T down to the wire width, then write the same little-endian
	// bytes. These are the only place the 32- and 64-bit byte order is pinned now that the unused
	// concrete writers are gone.
	[Theory]
	[MemberData(nameof(LittleEndianCases.UInt16), MemberType = typeof(LittleEndianCases))]
	public void Write16Bits_Narrows_T_And_Writes_Little_Endian(ushort value, byte[] expected)
	{
		using var stream = new MemoryStream();
		var writer = new ProtocolWriter<int>(stream);

		writer.Write16Bits(value);

		stream.ToArray().ShouldBe(expected);
	}

	[Theory]
	[MemberData(nameof(LittleEndianCases.UInt32), MemberType = typeof(LittleEndianCases))]
	public void Write32Bits_Narrows_T_And_Writes_Little_Endian(uint value, byte[] expected)
	{
		using var stream = new MemoryStream();
		var writer = new ProtocolWriter<long>(stream);

		writer.Write32Bits(value);

		stream.ToArray().ShouldBe(expected);
	}

	[Theory]
	[MemberData(nameof(LittleEndianCases.UInt64), MemberType = typeof(LittleEndianCases))]
	public void Write64Bits_Narrows_T_And_Writes_Little_Endian(ulong value, byte[] expected)
	{
		using var stream = new MemoryStream();
		var writer = new ProtocolWriter<ulong>(stream);

		writer.Write64Bits(value);

		stream.ToArray().ShouldBe(expected);
	}
}
