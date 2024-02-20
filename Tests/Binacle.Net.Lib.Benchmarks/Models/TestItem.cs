﻿using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Lib.Benchmarks.Models;

internal class TestItem : IItemWithDimensions<int>
{
    public TestItem()
    {

    }

    public string ID { get; set; }
    public int Quantity { get; set; }
    public int Length { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}
