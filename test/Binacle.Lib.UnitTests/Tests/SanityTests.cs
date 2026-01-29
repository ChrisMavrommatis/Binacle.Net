
namespace Binacle.Lib.UnitTests;

[Trait("Sanity Tests", "Ensures the tests are configured correctly")]
public class SanityTests : IClassFixture<CommonTestingFixture>
{
	private CommonTestingFixture Fixture { get; }

	public SanityTests(CommonTestingFixture fixture)
	{
		Fixture = fixture;
	}

	[Fact]
	public void Tests_Work()
	{
		true.ShouldBe(true);
	}
}
