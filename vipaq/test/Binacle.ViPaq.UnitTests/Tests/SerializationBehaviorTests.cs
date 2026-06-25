using Binacle.ViPaq.UnitTests.Models;

namespace Binacle.ViPaq.UnitTests;

// Invalid input is rejected rather than silently mishandled.
[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class SerializationBehaviorTests
{
	private static readonly Bin<int> ABin = new() { Length = 1, Width = 1, Height = 1 };
	private static readonly Item<int> AnItem = new() { Length = 1, Width = 1, Height = 1, X = 0, Y = 0, Z = 0 };

	[Fact]
	public void Serialize_Throws_When_Bin_Is_Null()
	{
		Should.Throw<ArgumentNullException>(() =>
			ViPaqSerializer.SerializeInt32<Bin<int>, Item<int>>(null!, [AnItem]));
	}

	[Fact]
	public void Serialize_Throws_When_Items_Are_Null()
	{
		Should.Throw<ArgumentNullException>(() =>
			ViPaqSerializer.SerializeInt32<Bin<int>, Item<int>>(ABin, null!));
	}

	[Fact]
	public void Deserialize_Throws_When_Data_Is_Null()
	{
		Should.Throw<ArgumentException>(() =>
			ViPaqSerializer.DeserializeInt32<Bin<int>, Item<int>>(null!));
	}

	[Fact]
	public void Deserialize_Throws_When_Data_Is_Empty()
	{
		Should.Throw<ArgumentException>(() =>
			ViPaqSerializer.DeserializeInt32<Bin<int>, Item<int>>([]));
	}

	// A header with a reserved version (10 or 11) is rejected when read back.
	[Theory]
	[InlineData((byte)0b10_00_00_00)] // Reserved2
	[InlineData((byte)0b11_00_00_00)] // Reserved3
	public void Deserialize_Throws_NotSupportedException_When_Version_Is_Reserved(byte header)
	{
		Should.Throw<NotSupportedException>(() =>
			ViPaqSerializer.DeserializeInt32<Bin<int>, Item<int>>([header]));
	}

	[Fact]
	public void Deserialize_Throws_When_Section_Is_Wider_Than_Type()
	{
		// Header says the bin is 64-bit, but we read it back as int (32-bit).
		const byte header = 0b00_11_00_00;

		Should.Throw<ArgumentOutOfRangeException>(() =>
			ViPaqSerializer.DeserializeInt32<Bin<int>, Item<int>>([header]));
	}
}
