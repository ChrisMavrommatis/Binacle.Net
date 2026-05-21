using System.Collections.Concurrent;
using Binacle.Lib.Abstractions;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;

namespace Binacle.Lib;

public class ParallelBinProcessor : IBinProcessor
{
	private readonly IAlgorithmFactory algorithmFactory;
	private readonly int concurrencyLevel;

	public ParallelBinProcessor(
		IAlgorithmFactory algorithmFactory,
		int? concurrencyLevel = null
		)
	{
		this.algorithmFactory = algorithmFactory;
		this.concurrencyLevel = concurrencyLevel ?? Environment.ProcessorCount;
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
			.StartActivity($"Process Bins: Parallel");
		activity?.SetTag("Operation", parameters.Operation);
		
		var results =  new ConcurrentDictionary<string, OperationResult>(this.concurrencyLevel, bins.Count);

		Parallel.For(0, bins.Count, i =>
		{
			var bin = bins[i];
			var algorithmInstance = this.algorithmFactory.Create(algorithm, bin, items);
			var result = algorithmInstance.Execute(parameters);
			results[bin.ID] = result;
		});

		return results;
	}
}
