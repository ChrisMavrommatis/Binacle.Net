using Binacle.Lib.Abstractions;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;

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

	public IDictionary<string, OperationResult> Process<TBin, TItem>(
		Algorithm algorithm,
		IList<TBin> bins,
		IList<TItem> items,
		IOperationParameters parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		using var activity = Diagnostics.ActivitySource
			.StartActivity($"Process Bins: Loop");
		activity?.SetTag("Operation", parameters.Operation);
		var results = new Dictionary<string, OperationResult>(bins.Count);

		for (var i = 0; i < bins.Count; i++)
		{
			var bin = bins[i];
			var algorithmInstance = this.algorithmFactory.Create(algorithm, bin, items);
			var result = algorithmInstance.Execute(parameters);
			results[bin.ID] = result;
		}

		return results;
	}
}
