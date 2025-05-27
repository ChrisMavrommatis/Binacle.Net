namespace Binacle.Net.TestsKernel.Benchmarks.Models;

public sealed class CubeScalingBenchmarkTestCase
{
	public CubeScalingBenchmarkTestCase(string itemString, int min, int max) :this(itemString, new(min, max))
	{
		
	}
	public CubeScalingBenchmarkTestCase(string itemString, TestsKernel.Models.Range range)
	{
		this.ItemString = itemString;
		this.Range = range;
	}

	public string ItemString { get; }
	public TestsKernel.Models.Range Range { get; }
}
