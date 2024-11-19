using BenchmarkDotNet.Attributes;
using Binacle.Net.Lib.Abstractions.Fitting;
using Binacle.Net.Lib.Fitting.Models;
using Binacle.Net.TestsKernel.Models;
using Binacle.Net.TestsKernel.Providers;

namespace Binacle.Net.Lib.Benchmarks.Combination;

[MemoryDiagnoser]
public class Fitting_FFD_MultipleBins
{
	private readonly BinCollectionsTestDataProvider binCollectionsDataProvider;

	public Fitting_FFD_MultipleBins()
	{
		this.binCollectionsDataProvider = new BinCollectionsTestDataProvider();
		this.scenarios = CombinationBenchmarkTestsDataProvider.Scenarios;
	}
	
	[Params(1, 2, 3)]
	public int AlgorithmVersion { get; set; }

	private readonly Dictionary<int, AlgorithmFactory<IFittingAlgorithm>> algorithmFactories = new()
	{
		{ 1, AlgorithmFactories.Fitting_FFD_v1 },
		{ 2, AlgorithmFactories.Fitting_FFD_v2 },
		{ 3, AlgorithmFactories.Fitting_FFD_v3 },
	};

	private readonly List<Scenario> scenarios;

	[Benchmark(Baseline = true)]
	public Dictionary<string, Lib.Fitting.Models.FittingResult> ForLoop()
	{
		var algorithmFactory = this.algorithmFactories[this.AlgorithmVersion];
		var results = new Dictionary<string, Lib.Fitting.Models.FittingResult>();
		
		foreach(var scenario in this.scenarios)
		{
			var bin = scenario.GetTestBin(this.binCollectionsDataProvider);
			
			var algorithmInstance = algorithmFactory(bin, scenario.Items);
			var result = algorithmInstance.Execute(new FittingParameters
			{
				ReportFittedItems = false,
				ReportUnfittedItems = false
			});
			results.Add(scenario.Name, result);
		}
		
		return results;
	}
}
