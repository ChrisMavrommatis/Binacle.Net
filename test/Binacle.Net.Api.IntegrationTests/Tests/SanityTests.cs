﻿using Xunit;

namespace Binacle.Net.Api.IntegrationTests;

[Trait("Sanity Tests", "Ensures the tests are configured correctly")]
public class SanityTests
{
	public SanityTests()
	{
	}

	[Fact]
	public void Tests_Work()
	{
		Assert.True(true);
	}
}
