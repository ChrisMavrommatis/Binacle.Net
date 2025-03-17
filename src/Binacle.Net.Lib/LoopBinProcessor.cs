using Binacle.Net.Lib.Abstractions;
using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Fitting.Models;
using Binacle.Net.Lib.Packing.Models;

namespace Binacle.Net.Lib;

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
		FittingParameters parameters
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

	public IDictionary<string, PackingResult> ProcessPacking<TBin, TItem>(
		Algorithm algorithm,
		IList<TBin> bins,
		IList<TItem> items,
		PackingParameters parameters
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
}
