﻿using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Lib.Benchmarks.Models;

internal class TestBin : IItemWithDimensions<int>
{
    public TestBin()
    {

    }

    public string ID { get; set; }
    public int Length { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}
