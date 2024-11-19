namespace Binacle.Net.TestsKernel.Models;

public sealed class ScalingBenchmarkTestCase
{
	public ScalingBenchmarkTestCase(string itemString, int min, int max) :this(itemString, new(min, max))
	{
		
	}
	public ScalingBenchmarkTestCase(string itemString, Range range)
	{
		this.ItemString = itemString;
		this.Range = range;
	}

	public string ItemString { get; }
	public Range Range { get; }
}
