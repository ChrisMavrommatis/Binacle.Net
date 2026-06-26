using Binacle.ViPaq.UnitTests.Models;

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
			Length = ValueFor(bitSize, 0),
			Width = ValueFor(bitSize, 1),
			Height = ValueFor(bitSize, 2),
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
			X = ValueFor(bitSize, 0),
			Y = ValueFor(bitSize, 1),
			Z = ValueFor(bitSize, 2),
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

	// A distinct value inside the size bucket. fieldIndex (0, 1, 2) shifts each of the three fields
	// to a different value, so a writer that swaps two fields shows up here as a mismatch.
	private static ulong ValueFor(BitSize size, int fieldIndex) => size switch
	{
		BitSize.Eight => 10UL + 10UL * (ulong)fieldIndex,                       // 10, 20, 30   (<= 255)
		BitSize.Sixteen => 300UL + 100UL * (ulong)fieldIndex,                   // 300, 400, 500
		BitSize.ThirtyTwo => 70_000UL + 1_000UL * (ulong)fieldIndex,            // 70000, 71000, 72000
		BitSize.SixtyFour => 5_000_000_000UL + 100_000_000UL * (ulong)fieldIndex, // ~5e9, +1e8 each
		_ => throw new ArgumentOutOfRangeException(nameof(size)),
	};
}
