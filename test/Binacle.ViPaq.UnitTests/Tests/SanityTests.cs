﻿using System.Numerics;
using Binacle.ViPaq.UnitTests.Models;

namespace Binacle.ViPaq.UnitTests;

[Trait("Sanity Tests", "Ensures the tests are configured correctly")]
public class SanityTests
{
    [Fact]
    public void Tests_Work()
    {
	    true.ShouldBe(true);
    }
}
