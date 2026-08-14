using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// The writer puts bytes on the wire little-endian. WriteUInt16 takes a fixed width and is used for the header.
// Write8Bits / Write16Bits narrow T to the wire width first. Reuses the same little-endian/<width>.json vectors
// as the reader tests, so both sides agree on the bytes.
[Trait("Result Tests", "Ensures results are as expected")]
public class ProtocolWriterTests
{
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

	// The only place the 16-bit byte order is pinned on the write side.
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
}
