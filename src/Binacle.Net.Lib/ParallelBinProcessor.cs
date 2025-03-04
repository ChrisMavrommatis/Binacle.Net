using System.Collections.Concurrent;
using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Fitting.Models;
using Binacle.Net.Lib.Packing.Models;

namespace Binacle.Net.Lib;

public class ParallelBinProcessor
{
	private readonly AlgorithmFactory algorithmFactory;

	public ParallelBinProcessor()
	{
		this.algorithmFactory = new AlgorithmFactory();
	}
	
	public ConcurrentDictionary<string, FittingResult> ProcessFitting<TBin, TItem>(
		Algorithm algorithm,
		IList<TBin> bins, 
		IList<TItem> items, 
		FittingParameters parameters,
		int? concurrencyLevel = null
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		using var activity = Diagnostics.ActivitySource
			.StartActivity("Process Fitting: Parallel");
		
		if(!concurrencyLevel.HasValue)
		{
			concurrencyLevel = Environment.ProcessorCount;
		}
		var results = new ConcurrentDictionary<string, FittingResult>(concurrencyLevel.Value, bins.Count);

		Parallel.For(0, bins.Count, i =>
		{
			var bin = bins[i];
			var algorithmInstance = this.algorithmFactory.CreateFitting(algorithm, bin, items);
			var result = algorithmInstance.Execute(parameters);
			results[bin.ID] = result;
		});

		return results;
	}

	public ConcurrentDictionary<string, PackingResult> ProcessPacking<TBin, TItem>(
		Algorithm algorithm,
		IList<TBin> bins, 
		IList<TItem> items, 
		PackingParameters parameters,
		int? concurrencyLevel = null
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		using var activity = Diagnostics.ActivitySource
			.StartActivity("Process Packing: Parallel");
		
		if(!concurrencyLevel.HasValue)
		{
			concurrencyLevel = Environment.ProcessorCount;
		}
		var results =  new ConcurrentDictionary<string, PackingResult>(concurrencyLevel.Value, bins.Count);

		Parallel.For(0, bins.Count, i =>
		{
			var bin = bins[i];
			var algorithmInstance = this.algorithmFactory.CreatePacking(algorithm, bin, items);
			var result = algorithmInstance.Execute(parameters);
			results[bin.ID] = result;
		});

		return results;
	}
	
}
