
namespace Binacle.ViPaq.UnitTests;

// A BitSize outside the four known values is rejected, not silently ignored.
[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class ProtocolExtensionsBehaviorTests
{
	private const BitSize Unsupported = (BitSize)99;

	[Fact]
	public void WriteDimensions_Throws_For_Unsupported_BitSize()
	{
		using var stream = new MemoryStream();
		var writer = new ProtocolWriter<int>(stream);
		var dimensions = new Dimensions<int> { Length = 1, Width = 1, Height = 1 };

		Should.Throw<ArgumentOutOfRangeException>(() =>
			writer.WriteDimensions<Dimensions<int>, int>(dimensions, Unsupported));
	}

	[Fact]
	public void WriteCoordinates_Throws_For_Unsupported_BitSize()
	{
		using var stream = new MemoryStream();
		var writer = new ProtocolWriter<int>(stream);
		var coordinates = new Coordinates<int> { X = 1, Y = 1, Z = 1 };

		Should.Throw<ArgumentOutOfRangeException>(() =>
			writer.WriteCoordinates<Coordinates<int>, int>(coordinates, Unsupported));
	}

	[Fact]
	public void ReadDimensions_Throws_For_Unsupported_BitSize()
	{
		var reader = new ProtocolReader<int>(new MemoryStream([1, 2, 3, 4]));
		var dimensions = new Dimensions<int>();

		Should.Throw<ArgumentOutOfRangeException>(() =>
			reader.ReadDimensions(ref dimensions, Unsupported));
	}

	[Fact]
	public void ReadCoordinates_Throws_For_Unsupported_BitSize()
	{
		var reader = new ProtocolReader<int>(new MemoryStream([1, 2, 3, 4]));
		var coordinates = new Coordinates<int>();

		Should.Throw<ArgumentOutOfRangeException>(() =>
			reader.ReadCoordinates(ref coordinates, Unsupported));
	}

	// 2^53 (one over MaxInteger) little-endian, in the low 8 bytes of a 24-byte (3-field) buffer.
	// A C#-made wire can carry a 64-bit value above the protocol ceiling; decoding must reject it (§5/§7).
	private static byte[] OneOverMaxIntegerSixtyFourField()
	{
		var bytes = new byte[24];
		new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00 }.CopyTo(bytes, 0);
		return bytes;
	}

	[Fact]
	public void ReadDimensions_Throws_When_A_SixtyFour_Field_Exceeds_MaxInteger()
	{
		var reader = new ProtocolReader<ulong>(new MemoryStream(OneOverMaxIntegerSixtyFourField()));
		var dimensions = new Dimensions<ulong>();

		Should.Throw<ArgumentOutOfRangeException>(() =>
			reader.ReadDimensions(ref dimensions, BitSize.SixtyFour));
	}

	[Fact]
	public void ReadCoordinates_Throws_When_A_SixtyFour_Field_Exceeds_MaxInteger()
	{
		var reader = new ProtocolReader<ulong>(new MemoryStream(OneOverMaxIntegerSixtyFourField()));
		var coordinates = new Coordinates<ulong>();

		Should.Throw<ArgumentOutOfRangeException>(() =>
			reader.ReadCoordinates(ref coordinates, BitSize.SixtyFour));
	}
}
