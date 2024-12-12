using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Fitting.Models;
using Binacle.Net.Lib.Packing.Models;

namespace Binacle.Net.Lib;

internal class ParallelBinProcessor
{
	private readonly AlgorithmFactory algorithmFactory;

	public ParallelBinProcessor()
	{
		this.algorithmFactory = new AlgorithmFactory();
	}
	
	public Dictionary<string, FittingResult> ProcessFitting<TBin, TItem>(
		Algorithm algorithm,
		IList<TBin> bins, 
		IList<TItem> items, 
		FittingParameters parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		var results = new Dictionary<string, FittingResult>(bins.Count);

		Parallel.ForEach(bins, bin =>
		{
			var algorithmInstance = this.algorithmFactory.CreateFitting(algorithm, bin, items);
			var result = algorithmInstance.Execute(parameters);
			results.Add(bin.ID, result);
		});


		return results;
	}

	public Dictionary<string, PackingResult> ProcessPacking<TBin, TItem>(
		Algorithm algorithm,
		IList<TBin> bins, 
		IList<TItem> items, 
		PackingParameters parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		var results = new Dictionary<string, PackingResult>(bins.Count);

		Parallel.ForEach(bins, bin =>
		{
			var algorithmInstance = this.algorithmFactory.CreatePacking(algorithm, bin, items);
			var result = algorithmInstance.Execute(parameters);
			results.Add(bin.ID, result);
		});

		return results;
	}
	
}
