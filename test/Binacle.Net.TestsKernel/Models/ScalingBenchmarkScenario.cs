using Binacle.Net.TestsKernel.Abstractions;
using Binacle.Net.TestsKernel.Helpers;
using Binacle.Net.TestsKernel.Providers;

namespace Binacle.Net.TestsKernel.Models;

public sealed class ScalingBenchmarkScenario : BinScenarioBase
{
	private readonly ScalingBenchmarkTestCase testCase;

	public string Name { get; }

	public int MaxInRange => this.testCase.Range.Max;

	public ScalingBenchmarkScenario(string binString, ScalingBenchmarkTestCase testCase) : base(binString)
	{
		this.testCase = testCase;
		this.Name = binString;
	}
	
	public override string ToString() => Name;

	public IEnumerable<int> GetNoOfItems()
	{
		var rangeDiff = this.testCase.Range.Max - this.testCase.Range.Min;

		var step = rangeDiff / (ScalingBenchmarkTestsDataProvider.TestsPerCase +1);


		// min
		yield return this.testCase.Range.Min;

		// med
		for(var i=1; i<= ScalingBenchmarkTestsDataProvider.TestsPerCase; i++)
		{
			yield return this.testCase.Range.Min + step * i;
		}

		// max
		yield return this.testCase.Range.Max;

		// over
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
