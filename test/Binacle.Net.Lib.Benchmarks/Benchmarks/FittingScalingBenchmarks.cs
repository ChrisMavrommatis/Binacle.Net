using BenchmarkDotNet.Attributes;
using Binacle.Net.Lib.Fitting.Models;
using Binacle.Net.Lib.Packing.Models;
using Binacle.Net.TestsKernel.Models;
using Binacle.Net.TestsKernel.Providers;
using System.Runtime.CompilerServices;

namespace Binacle.Net.Lib.Benchmarks;

[MemoryDiagnoser]
public class FittingScalingBenchmarks
{
	static string GetSolutionRoot([CallerFilePath] string callerFilePath = "")
	{
		//go ../../ from callerFilePath
		var callerDirectory = Path.GetDirectoryName(callerFilePath);
		var solutionRoot = Path.GetFullPath(Path.Combine(callerDirectory, "..", "..", ".."));
		return solutionRoot;
	}

	public FittingScalingBenchmarks()
	{
		this.binCollectionsDataProvider = new BinCollectionsTestDataProvider(solutionRootPath: GetSolutionRoot());

		// TODO Fix this
		this.scenario = BenchmarkScalingTestsDataProvider.Scenarios["Rectangular-Cuboids::Small"];
	}

	[ParamsSource(nameof(NoOfItemsParamsSourceAccessor))]
	public int NoOfItems { get; set; }

	public IEnumerable<int> NoOfItemsParamsSourceAccessor()
	{
		return this.scenario.GetNoOfItems();
	}

	private BinCollectionsTestDataProvider binCollectionsDataProvider;
	private BenchmarkScalingScenario scenario;
	private TestBin bin;
	private List<TestItem> items;

	[GlobalSetup]
	public void GlobalSetup()
	{
		this.bin = this.scenario.GetTestBin(this.binCollectionsDataProvider);
		this.items = this.scenario.GetTestItems(this.NoOfItems);

	}

	[GlobalCleanup]
	public void GlobalCleanup()
	{
		this.bin = null;
		this.items = null;
	}

	// TODO: Assert or test the results of the benchmarks
	[Benchmark(Baseline = true)]
	public FittingResult Fitting_FFD_V1()
	{
		var algorithmInstance = AlgorithmFactories.Fitting_FFD_v1(this.bin, this.items);
		var result = algorithmInstance.Execute(new FittingParameters { ReportFittedItems = false, ReportUnfittedItems = false});
		//BenchmarkScalingTestsDataProvider.AssertSuccessfulResult(result, NoOfItems);
		return result;
	}

	//[Benchmark]
	//public FittingResult Fitting_FFD_V1_Full()
	//{
	//	var algorithmInstance = AlgorithmFactories.Fitting_FFD_v1(this.bin, this.items);
	//	var result = algorithmInstance.Execute(new FittingParameters { ReportFittedItems = true, ReportUnfittedItems = true });
	//	//BenchmarkScalingTestsDataProvider.AssertSuccessfulResult(result, NoOfItems);
	//	return result;
	//}

	[Benchmark]
	public FittingResult Fitting_FFD_V2()
	{
		var algorithmInstance = AlgorithmFactories.Fitting_FFD_v2(this.bin, this.items);
		var result = algorithmInstance.Execute(new FittingParameters { ReportFittedItems = false, ReportUnfittedItems = false });
		//BenchmarkScalingTestsDataProvider.AssertSuccessfulResult(result, NoOfItems);
		return result;
	}

	//[Benchmark]
	//public FittingResult Fitting_FFD_V2_Full()
	//{
	//	var algorithmInstance = AlgorithmFactories.Fitting_FFD_v2(this.bin, this.items);
	//	var result = algorithmInstance.Execute(new FittingParameters { ReportFittedItems = true, ReportUnfittedItems = true });
	//	//BenchmarkScalingTestsDataProvider.AssertSuccessfulResult(result, NoOfItems);
	//	return result;
	//}

	[Benchmark]
	public FittingResult Fitting_FFD_V3()
	{
		var algorithmInstance = AlgorithmFactories.Fitting_FFD_v3(this.bin, this.items);
		var result = algorithmInstance.Execute(new FittingParameters { ReportFittedItems = false, ReportUnfittedItems = false });
		//BenchmarkScalingTestsDataProvider.AssertSuccessfulResult(result, NoOfItems);
		return result;
	}

	[Benchmark]
	public FittingResult Fitting_FFD_V4()
	{
		var algorithmInstance = AlgorithmFactories.Fitting_FFD_v4(this.bin, this.items);
		var result = algorithmInstance.Execute(new FittingParameters { ReportFittedItems = false, ReportUnfittedItems = false });
		//BenchmarkScalingTestsDataProvider.AssertSuccessfulResult(result, NoOfItems);
		return result;
	}

}
