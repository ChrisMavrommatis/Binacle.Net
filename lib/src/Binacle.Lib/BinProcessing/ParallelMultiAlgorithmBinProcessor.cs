using System.Collections.Concurrent;
using Binacle.Lib.Abstractions;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;

namespace Binacle.Lib;

public class ParallelMultiAlgorithmBinProcessor : IMultiAlgorithmBinProcessor
{
	private readonly IAlgorithmProcessor algorithmProcessor;
	private readonly IResultSelector resultSelector;
	private readonly int concurrencyLevel;

	public ParallelMultiAlgorithmBinProcessor(
		IAlgorithmProcessor algorithmProcessor,
		IResultSelector resultSelector,
		int? concurrencyLevel = null
	)
	{
		this.algorithmProcessor = algorithmProcessor;
		this.resultSelector = resultSelector;
		this.concurrencyLevel = concurrencyLevel ?? Environment.ProcessorCount;
	}

	public IDictionary<string, OperationResult> Process<TBin, TItem>(
		IList<TBin> bins,
		IList<TItem> items,
		IOperationParameters parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		using var activity = Diagnostics.ActivitySource
			.StartActivity($"Process Multi Algorithm Bins: Parallel");
		activity?.SetTag("Operation", parameters.Operation);

		var results = new ConcurrentDictionary<string, OperationResult>(this.concurrencyLevel, bins.Count);

		var parallelOptions = new ParallelOptions { CancellationToken = cancellationToken };
		Parallel.For(0, bins.Count, parallelOptions, i =>
		{
			var bin = bins[i];
			var algorithmResults = this.algorithmProcessor.Process(bin, items, parameters, cancellationToken);
			var selectedAlgorithmResult = this.resultSelector.BestAlgorithm(algorithmResults);
			results[bin.ID] = selectedAlgorithmResult;
		});

		return results;
	}
}
