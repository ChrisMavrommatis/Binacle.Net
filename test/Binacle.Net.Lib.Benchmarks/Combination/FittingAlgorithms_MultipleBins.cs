using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using Binacle.Net.Lib.Abstractions.Fitting;
using Binacle.Net.Lib.Fitting.Models;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Net.Lib.Benchmarks.Combination;

[MemoryDiagnoser]
public class FittingAlgorithms_MultipleBins : MultipleBinsBenchmarkBase
{
	[Params("FFD v1", "WFD v1", "BFD v1")]
	public string AlgorithmVersion { get; set; }

	private readonly Dictionary<string, AlgorithmFactory<IFittingAlgorithm>> algorithmFactories = new()
	{
		{ "FFD v1", AlgorithmFactories.Fitting_FFD_v1 },
		{ "WFD v1", AlgorithmFactories.Fitting_WFD_v1 },
		{ "BFD v1", AlgorithmFactories.Fitting_BFD_v1 }
	};

	[Benchmark(Baseline = true)]
	public Dictionary<string, FittingResult> ForeachLoop()
	{
		var algorithmFactory = this.algorithmFactories[this.AlgorithmVersion];
		var results = new Dictionary<string, FittingResult>(this.Bins.Count);

		foreach (var bin in this.Bins)
		{
			var algorithmInstance = algorithmFactory(bin, this.Items);
			var result = algorithmInstance.Execute(new FittingParameters()
			{
				ReportFittedItems = false,
				ReportUnfittedItems = false
			});
			results[bin.ID] = result;
		}

		return results;
	}

	[Benchmark]
	public Dictionary<string, FittingResult> ForLoop()
	{
		var algorithmFactory = this.algorithmFactories[this.AlgorithmVersion];
		var results = new Dictionary<string, FittingResult>(this.Bins.Count);

		for (int i = 0; i < this.Bins.Count; i++)
		{
			var bin = this.Bins[i];
			var algorithmInstance = algorithmFactory(bin, this.Items);
			var result = algorithmInstance.Execute(new FittingParameters()
			{
				ReportFittedItems = false,
				ReportUnfittedItems = false
			});
			results[bin.ID] = result;
		}

		return results;
	}

	[Benchmark]
	public Dictionary<string, FittingResult> SpanForLoop()
	{
		var algorithmFactory = this.algorithmFactories[this.AlgorithmVersion];
		var results = new Dictionary<string, FittingResult>(this.Bins.Count);

		var binsSpan = CollectionsMarshal.AsSpan(this.Bins);

		for (int i = 0; i < binsSpan.Length; i++)
		{
			var bin = binsSpan[i];
			var algorithmInstance = algorithmFactory(bin, this.Items);
			var result = algorithmInstance.Execute(new FittingParameters()
			{
				ReportFittedItems = false,
				ReportUnfittedItems = false
			});
			results[bin.ID] = result;
		}

		return results;
	}

	[Benchmark]
	public Dictionary<string, FittingResult> ParallelFor()
	{
		var algorithmFactory = this.algorithmFactories[this.AlgorithmVersion];
		var results = new Dictionary<string, FittingResult>(this.Bins.Count);

		Parallel.For(0, this.Bins.Count, index =>
		{
			var bin = this.Bins[index];
			var algorithmInstance = algorithmFactory(bin, this.Items);
			var result = algorithmInstance.Execute(new FittingParameters()
			{
				ReportFittedItems = false,
				ReportUnfittedItems = false
			});
			results[bin.ID] = result;
		});

		return results;
	}
}
