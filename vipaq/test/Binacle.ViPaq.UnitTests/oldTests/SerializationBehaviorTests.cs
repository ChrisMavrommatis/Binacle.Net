// using Binacle.ViPaq.UnitTests.Models;
//
// namespace Binacle.ViPaq.UnitTests;
//
// // Purpose: invalid input is rejected rather than silently mishandled.
// [Trait("Behavioral Tests", "Ensures operations behave as expected")]
// public class SerializationBehaviorTests : IClassFixture<SerializationTestingFixture>
// {
// 	private readonly SerializationTestingFixture fixture;
//
// 	public SerializationBehaviorTests(SerializationTestingFixture fixture)
// 	{
// 		this.fixture = fixture;
// 	}
//
// 	[Fact]
// 	public void Serialize_Throws_When_Bin_Is_Null()
// 	{
// 		var items = this.fixture.CreateItems(1, 255, 0, 255, 1);
//
// 		Should.Throw<ArgumentNullException>(() => this.fixture.SerializeInt32(null!, items));
// 	}
//
// 	[Fact]
// 	public void Serialize_Throws_When_Items_Are_Null()
// 	{
// 		var bin = this.fixture.CreateBin(1, 255);
//
// 		Should.Throw<ArgumentNullException>(() => this.fixture.SerializeInt32(bin, null!));
// 	}
//
// 	[Fact]
// 	public void Deserialize_Throws_When_Data_Is_Null()
// 	{
// 		Should.Throw<ArgumentException>(() => this.fixture.DeserializeInt32(null!));
// 	}
//
// 	[Fact]
// 	public void Deserialize_Throws_When_Data_Is_Empty()
// 	{
// 		Should.Throw<ArgumentException>(() => this.fixture.DeserializeInt32(Array.Empty<byte>()));
// 	}
// }
