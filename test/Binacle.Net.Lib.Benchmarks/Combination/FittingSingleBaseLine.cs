using BenchmarkDotNet.Attributes;
using Binacle.Net.Lib.Fitting.Models;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Net.Lib.Benchmarks.Combination;

[MemoryDiagnoser]
public class FittingSingleBaseLine : MultipleBinsBenchmarkBase
{
	private readonly TestBin testBin;
	private readonly AlgorithmFactory algorithmFactory;

	public FittingSingleBaseLine()
	{
		// .Add("Rectangular-Cuboids::Medium", "BinaryDecision::Fits")
		this.algorithmFactory = new AlgorithmFactory();
		this.testBin = this.DataProvider
			.GetSuccessfulBins(this.BinCollectionsDataProvider)
			.FirstOrDefault(x => x.ID == "Medium")!;
		this.Items = this.DataProvider.GetItems();
	}

	public List<TestItem> Items { get; set; }

	[Benchmark(Baseline = true)]
	public FittingResult FFD()
	{
		var algorithmInstance = this.algorithmFactory.CreateFitting(Algorithm.FirstFitDecreasing, this.testBin, this.Items);
		var result = algorithmInstance.Execute(new FittingParameters
		{
			ReportFittedItems = false,
			ReportUnfittedItems = false
		});
		return result;
		
	}
	[Benchmark]
	public FittingResult WFD()
	{
		var algorithmInstance = this.algorithmFactory.CreateFitting(Algorithm.WorstFitDecreasing, this.testBin, this.Items);
		var result = algorithmInstance.Execute(new FittingParameters
		{
			ReportFittedItems = false,
			ReportUnfittedItems = false
		});
		return result;
		
	}
	[Benchmark]
	public FittingResult BFD()
	{
		var algorithmInstance = this.algorithmFactory.CreateFitting(Algorithm.BestFitDecreasing, this.testBin, this.Items);
		var result = algorithmInstance.Execute(new FittingParameters
		{
			ReportFittedItems = false,
			ReportUnfittedItems = false
		});
		return result;
	}
}
