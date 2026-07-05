
namespace Binacle.ViPaq.UnitTests;

// Invalid input is rejected rather than silently mishandled.
[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class SerializationBehaviorTests
{
	private static readonly Binacle.Geometry.Dimensions<int> ABin = new() { Length = 1, Width = 1, Height = 1 };
	private static readonly Binacle.Geometry.Item<int> AnItem = new() { Length = 1, Width = 1, Height = 1, X = 0, Y = 0, Z = 0 };

	[Fact]
	public void Serialize_Throws_When_Bin_Is_Null()
	{
		var exception = Should.Throw<ArgumentNullException>(() =>
			ViPaqSerializer.SerializeInt32<Binacle.Geometry.Dimensions<int>, Binacle.Geometry.Item<int>>(null!, [AnItem]));
		exception.ParamName.ShouldBe("bin");
	}

	[Fact]
	public void Serialize_Throws_When_Items_Are_Null()
	{
		var exception = Should.Throw<ArgumentNullException>(() =>
			ViPaqSerializer.SerializeInt32<Binacle.Geometry.Dimensions<int>, Binacle.Geometry.Item<int>>(ABin, null!));
		exception.ParamName.ShouldBe("items");
	}

	[Fact]
	public void Deserialize_Throws_When_Data_Is_Null()
	{
		var exception = Should.Throw<ArgumentException>(() =>
			ViPaqSerializer.DeserializeInt32<Binacle.Geometry.Dimensions<int>, Binacle.Geometry.Item<int>>(null!));
		exception.ParamName.ShouldBe("data");
	}

	[Fact]
	public void Deserialize_Throws_When_Data_Is_Empty()
	{
		var exception = Should.Throw<ArgumentException>(() =>
			ViPaqSerializer.DeserializeInt32<Binacle.Geometry.Dimensions<int>, Binacle.Geometry.Item<int>>([]));
		exception.ParamName.ShouldBe("data");
	}

	// A header with a reserved version (10 or 11) is rejected when read back.
	[Theory]
	[InlineData((byte)0b10_00_00_00)] // Reserved2
	[InlineData((byte)0b11_00_00_00)] // Reserved3
	public void Deserialize_Throws_NotSupportedException_When_Version_Is_Reserved(byte header)
	{
		Should.Throw<NotSupportedException>(() =>
			ViPaqSerializer.DeserializeInt32<Binacle.Geometry.Dimensions<int>, Binacle.Geometry.Item<int>>([header]));
	}

	[Fact]
	public void Deserialize_Throws_When_Section_Is_Wider_Than_Type()
	{
		// Header says the bin is 64-bit, but we read it back as int (32-bit).
		const byte header = 0b00_11_00_00;

		// ParamName points at the bin section that was too wide, not some later check.
		var exception = Should.Throw<ArgumentOutOfRangeException>(() =>
			ViPaqSerializer.DeserializeInt32<Binacle.Geometry.Dimensions<int>, Binacle.Geometry.Item<int>>([header]));
		exception.ParamName.ShouldBe(nameof(EncodingInfo.BinDimensionsBitSize));
	}
}
