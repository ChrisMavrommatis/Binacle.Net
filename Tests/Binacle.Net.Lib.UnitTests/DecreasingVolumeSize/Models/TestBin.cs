using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Lib.UnitTests.DecreasingVolumeSize.Models;

public sealed class TestBin : IItemWithDimensions<int>
{
    public TestBin()
    {

    }

    public TestBin(string id, IWithReadOnlyDimensions<int> item)
    {
        this.ID = id;
        this.Length = item.Length;
        this.Width = item.Width;
        this.Height = item.Height;
    }
    public string ID { get; set; }
    public int Length { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}
