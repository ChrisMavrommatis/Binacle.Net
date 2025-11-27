using Binacle.Lib.Abstractions;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Fitting;
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Fitting.Models;
using Binacle.Lib.Packing.Models;

namespace Binacle.Lib;

public class LoopBinProcessor : IBinProcessor
{
	private readonly IAlgorithmFactory algorithmFactory;

	public LoopBinProcessor(
		IAlgorithmFactory algorithmFactory
		)
	{
		this.algorithmFactory = algorithmFactory;
	}

	public IDictionary<string, FittingResult> ProcessFitting<TBin, TItem>(
		Algorithm algorithm,
		IList<TBin> bins,
		IList<TItem> items,
		IFittingParameters parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		using var activity = Diagnostics.ActivitySource
			.StartActivity("Process Fitting: Loop");
		
		var results = new Dictionary<string, FittingResult>(bins.Count);

		for (var i = 0; i < bins.Count; i++)
		{
			var bin = bins[i];
			var algorithmInstance = this.algorithmFactory.CreateFitting(algorithm, bin, items);
			var result = algorithmInstance.Execute(parameters);
			results[bin.ID] = result;
		}

		return results;
	}
	
	// public IDictionary<string, FittingResult[]> ProcessFitting<TBin, TItem>(
	// 	IList<TBin> bins,
	// 	IList<TItem> items,
	// 	IFittingParameters parameters
	// )
	// 	where TBin : class, IWithID, IWithReadOnlyDimensions
	// 	where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	// {
	// 	using var activity = Diagnostics.ActivitySource
	// 		.StartActivity("Process Fitting: Loop");
	// 	
	// 	var results = new Dictionary<string, FittingResult[]>(bins.Count);
	//
	// 	for (var i = 0; i < bins.Count; i++)
	// 	{
	// 		var bin = bins[i];
	// 		var algorithmInstances = this.algorithmFactory.CreateFitting(bin, items);
	// 		var algorithmResults = new FittingResult[algorithmInstances.Length];
	// 		for(var j = 0; j < algorithmInstances.Length; j++)
	// 		{
	// 			var algorithmInstance = algorithmInstances[j];
	// 			algorithmResults[j] = algorithmInstance.Execute(parameters);
	// 		}
	// 		results[bin.ID] = algorithmResults;
	// 	}
	//
	// 	return results;
	// }

	public IDictionary<string, PackingResult> ProcessPacking<TBin, TItem>(
		Algorithm algorithm,
		IList<TBin> bins,
		IList<TItem> items,
		IPackingParameters parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		using var activity = Diagnostics.ActivitySource
			.StartActivity("Process Packing: Loop");
		
		var results = new Dictionary<string, PackingResult>(bins.Count);

		for (var i = 0; i < bins.Count; i++)
		{
			var bin = bins[i];
			var algorithmInstance = this.algorithmFactory.CreatePacking(algorithm, bin, items);
			var result = algorithmInstance.Execute(parameters);
			results[bin.ID] = result;
		}

		return results;
	}
	
	// public IDictionary<string, PackingResult[]> ProcessPacking<TBin, TItem>(
	// 	IList<TBin> bins,
	// 	IList<TItem> items,
	// 	IPackingParameters parameters
	// )
	// 	where TBin : class, IWithID, IWithReadOnlyDimensions
	// 	where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	// {
	// 	using var activity = Diagnostics.ActivitySource
	// 		.StartActivity("Process Packing: Loop");
	// 	
	// 	var results = new Dictionary<string, PackingResult[]>(bins.Count);
	//
	// 	for (var i = 0; i < bins.Count; i++)
	// 	{
	// 		var bin = bins[i];
	// 		var algorithmInstances = this.algorithmFactory.CreatePacking(bin, items);
	// 		var algorithmResults = new PackingResult[algorithmInstances.Length];
	// 		for(var j = 0; j < algorithmInstances.Length; j++)
	// 		{
	// 			var algorithmInstance = algorithmInstances[j];
	// 			algorithmResults[j] = algorithmInstance.Execute(parameters);
	// 		}
	// 		results[bin.ID] = algorithmResults;
	// 	}
	//
	// 	return results;
	// }
}
