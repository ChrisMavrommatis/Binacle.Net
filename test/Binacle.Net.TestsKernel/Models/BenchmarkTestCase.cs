namespace Binacle.Net.TestsKernel.Models;

public sealed class BenchmarkTestCase
{
	public BenchmarkTestCase(string itemString, int min, int max) :this(itemString, new(min, max))
	{
		
	}
	public BenchmarkTestCase(string itemString, Range range)
	{
		this.ItemString = itemString;
		this.Range = range;
	}

	public string ItemString { get; }
	public Range Range { get; }
}
