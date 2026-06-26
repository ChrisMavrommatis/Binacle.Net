using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// The writer puts bytes on the wire little-endian (low byte first). The WriteAs* methods narrow T
// down to the wire width first, then write the same bytes. These pin the exact byte order, and they
// reuse the same vectors as the reader tests (see LittleEndianCases) so both sides agree on the bytes.
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

	[Theory]
	[MemberData(nameof(LittleEndianCases.UInt32), MemberType = typeof(LittleEndianCases))]
	public void WriteUInt32_Writes_Little_Endian(uint value, byte[] expected)
	{
		using var stream = new MemoryStream();
		var writer = new ProtocolWriter<int>(stream);

		writer.WriteUInt32(value);

		stream.ToArray().ShouldBe(expected);
	}

	[Theory]
	[MemberData(nameof(LittleEndianCases.UInt64), MemberType = typeof(LittleEndianCases))]
	public void WriteUInt64_Writes_Little_Endian(ulong value, byte[] expected)
	{
		using var stream = new MemoryStream();
		var writer = new ProtocolWriter<ulong>(stream);

		writer.WriteUInt64(value);

		stream.ToArray().ShouldBe(expected);
	}

	// input byte -> same byte out, narrowed from T first.
	[Theory]
	[InlineData(0x00)]
	[InlineData(0xAB)]
	[InlineData(0xFF)]
	public void WriteAsByte_Narrows_T_And_Writes(byte value)
	{
		using var stream = new MemoryStream();
		var writer = new ProtocolWriter<int>(stream);

		writer.WriteAsByte(value);

		stream.ToArray().ShouldBe(new[] { value });
	}

	// The WriteAs* methods narrow T down to the wire width, then write the same little-endian bytes.
	[Theory]
	[MemberData(nameof(LittleEndianCases.UInt16), MemberType = typeof(LittleEndianCases))]
	public void WriteAsUInt16_Narrows_T_And_Writes_Little_Endian(ushort value, byte[] expected)
	{
		using var stream = new MemoryStream();
		var writer = new ProtocolWriter<int>(stream);

		writer.WriteAsUInt16(value);

		stream.ToArray().ShouldBe(expected);
	}

	[Theory]
	[MemberData(nameof(LittleEndianCases.UInt32), MemberType = typeof(LittleEndianCases))]
	public void WriteAsUInt32_Narrows_T_And_Writes_Little_Endian(uint value, byte[] expected)
	{
		using var stream = new MemoryStream();
		var writer = new ProtocolWriter<long>(stream);

		writer.WriteAsUInt32(value);

		stream.ToArray().ShouldBe(expected);
	}

	[Theory]
	[MemberData(nameof(LittleEndianCases.UInt64), MemberType = typeof(LittleEndianCases))]
	public void WriteAsUInt64_Narrows_T_And_Writes_Little_Endian(ulong value, byte[] expected)
	{
		using var stream = new MemoryStream();
		var writer = new ProtocolWriter<ulong>(stream);

		writer.WriteAsUInt64(value);

		stream.ToArray().ShouldBe(expected);
	}
}
