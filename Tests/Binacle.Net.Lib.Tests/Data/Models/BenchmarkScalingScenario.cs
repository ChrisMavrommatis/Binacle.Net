namespace Binacle.Net.Lib.Tests.Data.Models;

public sealed class BenchmarkScalingScenario
{
	public BenchmarkScalingScenario()
	{
		

	}

	public BenchmarkScalingScenario(int noOfItems, string expectedSize)
	{
		this.NoOfItems = noOfItems;
		this.ExpectedSize = expectedSize;
	}
	public int NoOfItems { get; set; }
	public string ExpectedSize { get; set; }
}
