using Xunit;

namespace Binacle.Net.Api.IntegrationTests.Tests;

[Trait("Sanity Tests", "Ensures the tests are configured correctly")]
public class Sanity
{
	public Sanity()
	{
	}

	[Fact]
	public void Tests_Work()
	{
		Xunit.Assert.True(true);
	}
}
