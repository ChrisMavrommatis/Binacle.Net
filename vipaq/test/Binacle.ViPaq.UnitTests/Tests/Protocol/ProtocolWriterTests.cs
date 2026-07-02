using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// The writer puts bytes on the wire little-endian (low byte first). WriteByte / WriteUInt16 take a
// fixed width and are used for the header. Write8Bits..Write64Bits narrow T down to the wire width
// first, then write the same bytes. These pin the exact byte order, and they reuse the shared
// little-endian/<width>.json vectors as the reader tests so both sides agree on the bytes.
[Trait("Result Tests", "Ensures results are as expected")]
public class ProtocolWriterTests
{
	// input byte -> same byte out. No endianness to worry about for a single byte.
	[Theory]
	[MemberData(nameof(LittleEndianProvider.UInt8Names), MemberType = typeof(LittleEndianProvider))]
	public void WriteByte_Writes_The_Byte(string name)
	{
		var (value, expected) = LittleEndianProvider.UInt8(name);
		using var stream = new MemoryStream();
		var writer = new ProtocolWriter<int>(stream);

		writer.WriteByte(value);

		stream.ToArray().ShouldBe(expected);
	}

	// value in -> the little-endian bytes it occupies on the wire (low byte first).
	[Theory]
	[MemberData(nameof(LittleEndianProvider.UInt16Names), MemberType = typeof(LittleEndianProvider))]
	public void WriteUInt16_Writes_Little_Endian(string name)
	{
		var (value, expected) = LittleEndianProvider.UInt16(name);
		using var stream = new MemoryStream();
		var writer = new ProtocolWriter<int>(stream);

		writer.WriteUInt16(value);

		stream.ToArray().ShouldBe(expected);
	}

	// input byte -> same byte out, narrowed from T first.
	[Theory]
	[MemberData(nameof(LittleEndianProvider.UInt8Names), MemberType = typeof(LittleEndianProvider))]
	public void Write8Bits_Narrows_T_And_Writes(string name)
	{
		var (value, expected) = LittleEndianProvider.UInt8(name);
		using var stream = new MemoryStream();
		var writer = new ProtocolWriter<int>(stream);

		writer.Write8Bits(value);

		stream.ToArray().ShouldBe(expected);
	}

	// Write16Bits..Write64Bits narrow T down to the wire width, then write the same little-endian
	// bytes. These are the only place the 32- and 64-bit byte order is pinned now that the unused
	// concrete writers are gone.
	[Theory]
	[MemberData(nameof(LittleEndianProvider.UInt16Names), MemberType = typeof(LittleEndianProvider))]
	public void Write16Bits_Narrows_T_And_Writes_Little_Endian(string name)
	{
		var (value, expected) = LittleEndianProvider.UInt16(name);
		using var stream = new MemoryStream();
		var writer = new ProtocolWriter<int>(stream);

		writer.Write16Bits(value);

		stream.ToArray().ShouldBe(expected);
	}

	[Theory]
	[MemberData(nameof(LittleEndianProvider.UInt32Names), MemberType = typeof(LittleEndianProvider))]
	public void Write32Bits_Narrows_T_And_Writes_Little_Endian(string name)
	{
		var (value, expected) = LittleEndianProvider.UInt32(name);
		using var stream = new MemoryStream();
		var writer = new ProtocolWriter<long>(stream);

		writer.Write32Bits(value);

		stream.ToArray().ShouldBe(expected);
	}

	[Theory]
	[MemberData(nameof(LittleEndianProvider.UInt64Names), MemberType = typeof(LittleEndianProvider))]
	public void Write64Bits_Narrows_T_And_Writes_Little_Endian(string name)
	{
		var (value, expected) = LittleEndianProvider.UInt64(name);
		using var stream = new MemoryStream();
		var writer = new ProtocolWriter<ulong>(stream);

		writer.Write64Bits(value);

		stream.ToArray().ShouldBe(expected);
	}
}
