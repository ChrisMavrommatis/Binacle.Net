namespace Binacle.CompactNotation.UnitTests;

public class DetectAndRoundTripTests
{
	[Theory]
	[InlineData("(1,2,3)", CompactNotationKind.Coordinates)]
	[InlineData("[5]", CompactNotationKind.Quantity)]
	[InlineData("10x20x30", CompactNotationKind.Dimensions)]
	[InlineData(" (1,2,3)", CompactNotationKind.Coordinates)] // leading space is fine
	public void Detect_picks_the_block_from_the_leading_token(string compact, CompactNotationKind expected)
	{
		CompactNotationParser.Detect(compact).ShouldBe(expected);
	}

	[Fact]
	public void Detect_rejects_an_unknown_string()
	{
		Should.Throw<FormatException>(() => CompactNotationParser.Detect("nonsense"));
	}

	[Theory]
	[InlineData("10x20x30")]
	[InlineData("1x1x1")]
	public void Dimensions_round_trip(string compact)
	{
		var dimensions = CompactNotationParser.ParseDimensions<long>(compact);

		CompactNotationFormatter.FormatDimensions(dimensions).ShouldBe(compact);
	}

	[Theory]
	[InlineData("(1,2,3)")]
	[InlineData("(0,0,0)")]
	public void Coordinates_round_trip(string compact)
	{
		var coordinates = CompactNotationParser.ParseCoordinates<long>(compact);

		CompactNotationFormatter.FormatCoordinates(coordinates).ShouldBe(compact);
	}

	[Theory]
	[InlineData("10x20x30 (1,2,3)")]
	public void Item_round_trip(string compact)
	{
		var item = CompactNotationParser.ParseItem<long>(compact);

		CompactNotationFormatter.Format<long>(item).ShouldBe(compact);
	}
}
