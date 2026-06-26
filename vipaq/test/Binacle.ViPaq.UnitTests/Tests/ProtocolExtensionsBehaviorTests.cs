using Binacle.ViPaq.UnitTests.Models;

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
}
