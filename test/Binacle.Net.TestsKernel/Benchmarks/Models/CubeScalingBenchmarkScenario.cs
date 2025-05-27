using Binacle.Net.TestsKernel.Abstractions;
using Binacle.Net.TestsKernel.Helpers;

namespace Binacle.Net.TestsKernel.Benchmarks.Models;

public sealed class CubeScalingBenchmarkScenario : BinScenarioBase
{
	private readonly CubeScalingBenchmarkTestCase testCase;

	public string Name { get; }

	public int MaxInRange => this.testCase.Range.Max;

	public CubeScalingBenchmarkScenario(string binString, CubeScalingBenchmarkTestCase testCase) : base(binString)
	{
		this.testCase = testCase;
		this.Name = binString;
	}
	
	public override string ToString() => Name;

	public IEnumerable<int> GetNoOfItems()
	{
		var rangeDiff = this.testCase.Range.Max - this.testCase.Range.Min;

		var step = rangeDiff / (CubeScalingBenchmarkTestsDataProvider.TestsPerCase +1);


		// min
		yield return this.testCase.Range.Min;

		// med
		for(var i=1; i<= CubeScalingBenchmarkTestsDataProvider.TestsPerCase; i++)
		{
			yield return this.testCase.Range.Min + step * i;
		}

		// max
		yield return this.testCase.Range.Max;

		// over
		yield return this.testCase.Range.Max + this.testCase.Range.Min;
	}

	public List<TestsKernel.Models.TestItem> GetTestItems(int noOfItems)
	{
		var dimensions = DimensionHelper.ParseFromCompactString(this.testCase.ItemString);
		return [
			new TestsKernel.Models.TestItem(this.testCase.ItemString, dimensions, noOfItems)
		];
	}
}
