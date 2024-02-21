namespace Binacle.Net.Lib.Tests.Models;

internal class Range
{
    public Range(int min, int max)
    {
        this.Min = min;
        this.Max = max;
    }

    public int Min { get; set; }
    public int Max { get; set; }

    public bool IsWithin(int value)
    {
        return value >= this.Min && value <= this.Max;
    }
}

