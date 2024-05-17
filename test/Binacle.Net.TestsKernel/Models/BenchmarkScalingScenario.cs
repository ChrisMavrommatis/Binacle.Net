namespace Binacle.Net.TestsKernel.Models;

public sealed class BenchmarkScalingScenario
{
	public BenchmarkScalingScenario()
	{


	}

	public BenchmarkScalingScenario(int noOfItems, string expectedSize)
	{
		NoOfItems = noOfItems;
		ExpectedSize = expectedSize;
	}
	public int NoOfItems { get; set; }
	public string ExpectedSize { get; set; }
}
