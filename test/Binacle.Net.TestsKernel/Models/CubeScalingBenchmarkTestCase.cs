namespace Binacle.Net.TestsKernel.Models;

public sealed class CubeScalingBenchmarkTestCase
{
	public CubeScalingBenchmarkTestCase(string itemString, int min, int max) :this(itemString, new(min, max))
	{
		
	}
	public CubeScalingBenchmarkTestCase(string itemString, Range range)
	{
		this.ItemString = itemString;
		this.Range = range;
	}

	public string ItemString { get; }
	public Range Range { get; }
}
