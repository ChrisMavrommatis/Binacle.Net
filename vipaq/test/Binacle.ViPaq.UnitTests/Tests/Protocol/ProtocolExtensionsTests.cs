
namespace Binacle.ViPaq.UnitTests;

// The extensions write/read the three fields in order at the chosen width. Round-tripping each
// BitSize proves the width branches line up between writer and reader.
[Trait("Result Tests", "Ensures results are as expected")]
public class ProtocolExtensionsTests
{
	[Theory]
	[InlineData(BitSize.Eight)]
	[InlineData(BitSize.Sixteen)]
	[InlineData(BitSize.ThirtyTwo)]
	[InlineData(BitSize.SixtyFour)]
	public void WriteDimensions_Then_ReadDimensions_Round_Trips(BitSize bitSize)
	{
		var source = new Dimensions<ulong>
		{
			Length = BitSizeValues.DistinctValue<ulong>(bitSize, 0),
			Width = BitSizeValues.DistinctValue<ulong>(bitSize, 1),
			Height = BitSizeValues.DistinctValue<ulong>(bitSize, 2),
		};

		using var stream = new MemoryStream();
		var writer = new ProtocolWriter<ulong>(stream);
		writer.WriteDimensions<Dimensions<ulong>, ulong>(source, bitSize);

		stream.Position = 0;
		var target = new Dimensions<ulong>();
		var reader = new ProtocolReader<ulong>(stream);
		reader.ReadDimensions(ref target, bitSize);

		target.Length.ShouldBe(source.Length);
		target.Width.ShouldBe(source.Width);
		target.Height.ShouldBe(source.Height);
	}

	[Theory]
	[InlineData(BitSize.Eight)]
	[InlineData(BitSize.Sixteen)]
	[InlineData(BitSize.ThirtyTwo)]
	[InlineData(BitSize.SixtyFour)]
	public void WriteCoordinates_Then_ReadCoordinates_Round_Trips(BitSize bitSize)
	{
		var source = new Coordinates<ulong>
		{
			X = BitSizeValues.DistinctValue<ulong>(bitSize, 0),
			Y = BitSizeValues.DistinctValue<ulong>(bitSize, 1),
			Z = BitSizeValues.DistinctValue<ulong>(bitSize, 2),
		};

		using var stream = new MemoryStream();
		var writer = new ProtocolWriter<ulong>(stream);
		writer.WriteCoordinates<Coordinates<ulong>, ulong>(source, bitSize);

		stream.Position = 0;
		var target = new Coordinates<ulong>();
		var reader = new ProtocolReader<ulong>(stream);
		reader.ReadCoordinates(ref target, bitSize);

		target.X.ShouldBe(source.X);
		target.Y.ShouldBe(source.Y);
		target.Z.ShouldBe(source.Z);
	}
}
