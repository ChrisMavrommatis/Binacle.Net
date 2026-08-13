namespace Binacle.ViPaq.UnitTests;

// Invalid input is rejected, not silently mishandled. Encode rejections are argument errors, decode rejections
// are ViPaqFormatException.
[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class SerializationBehaviorTests
{
	private static readonly Binacle.Geometry.Dimensions<int> ABin = new() { Length = 1, Width = 1, Height = 1 };
	private static readonly Binacle.Geometry.Item<int> AnItem = new() { Length = 1, Width = 1, Height = 1, X = 0, Y = 0, Z = 0 };

	[Fact]
	public void Serialize_Throws_When_Bin_Is_Null()
	{
		var exception = Should.Throw<ArgumentNullException>(() =>
			ViPaqSerializer.Serialize<Binacle.Geometry.Dimensions<int>, Binacle.Geometry.Item<int>, int>(null!, [AnItem]));
		exception.ParamName.ShouldBe("bin");
	}

	[Fact]
	public void Serialize_Throws_When_Items_Are_Null()
	{
		var exception = Should.Throw<ArgumentNullException>(() =>
			ViPaqSerializer.Serialize<Binacle.Geometry.Dimensions<int>, Binacle.Geometry.Item<int>, int>(ABin, null!));
		exception.ParamName.ShouldBe("items");
	}

	// A blob is at least two bytes (the header), so null and empty are malformed, not argument errors.
	[Fact]
	public void Deserialize_Throws_When_Data_Is_Null()
	{
		Should.Throw<ViPaqFormatException>(() =>
			ViPaqSerializer.Deserialize<Binacle.Geometry.Dimensions<int>, Binacle.Geometry.Item<int>, int>(null!));
	}

	[Fact]
	public void Deserialize_Throws_When_Data_Is_Empty()
	{
		Should.Throw<ViPaqFormatException>(() =>
			ViPaqSerializer.Deserialize<Binacle.Geometry.Dimensions<int>, Binacle.Geometry.Item<int>, int>([]));
	}

	// A reserved version (10 or 11) is rejected when read back. The second byte is the all-zero widths byte, so
	// the blob is a whole two-byte header.
	[Theory]
	[InlineData((byte)0b10_00_00_00)] // Reserved2
	[InlineData((byte)0b11_00_00_00)] // Reserved3
	public void Deserialize_Throws_When_Version_Is_Reserved(byte formByte)
	{
		Should.Throw<ViPaqFormatException>(() =>
			ViPaqSerializer.Deserialize<Binacle.Geometry.Dimensions<int>, Binacle.Geometry.Item<int>, int>([formByte, 0]));
	}

	// A reserved width code never reaches the wire, so reading one back is a malformed blob.
	[Fact]
	public void Deserialize_Throws_When_Width_Code_Is_Reserved()
	{
		byte[] blob = [0b00_00_00_00, 0b11_00_00_00];

		Should.Throw<ViPaqFormatException>(() =>
			ViPaqSerializer.Deserialize<Binacle.Geometry.Dimensions<int>, Binacle.Geometry.Item<int>, int>(blob));
	}

	[Fact]
	public void Deserialize_Throws_When_Section_Is_Wider_Than_Type()
	{
		// Header declares a 16-bit bin (widths byte bits 7-6 = 01), but we read it back as byte (8-bit).
		byte[] blob = [0b00_00_00_00, 0b01_00_00_00];

		// ParamName points at the bin section that was too wide, not some later check.
		var exception = Should.Throw<ArgumentOutOfRangeException>(() =>
			ViPaqSerializer.Deserialize<Binacle.Geometry.Dimensions<byte>, Binacle.Geometry.Item<byte>, byte>(blob));
		exception.ParamName.ShouldBe(nameof(Header.BinDimensionsWidth));
	}
}
