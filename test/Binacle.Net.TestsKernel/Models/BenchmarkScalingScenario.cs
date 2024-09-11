using Binacle.Net.TestsKernel.Helpers;

namespace Binacle.Net.TestsKernel.Models;

public sealed class BenchmarkScalingScenario : BinScenarioBase
{
	private readonly BenchmarkTestCase testCase;

	public BenchmarkScalingScenario(string binString, BenchmarkTestCase testCase) : base(binString)
	{
		this.testCase = testCase;
	}

	public IEnumerable<int> GetNoOfItems()
	{
		yield return this.testCase.Range.Min;
		yield return this.testCase.Range.Max;
		yield return this.testCase.Range.Max + this.testCase.Range.Min;
	}

	public List<TestItem> GetTestItems(int noOfItems)
	{
		var dimensions = DimensionHelper.ParseFromCompactString(this.testCase.ItemString);
		return [
			new TestItem(this.testCase.ItemString, dimensions, noOfItems)
		];
	}
}


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
