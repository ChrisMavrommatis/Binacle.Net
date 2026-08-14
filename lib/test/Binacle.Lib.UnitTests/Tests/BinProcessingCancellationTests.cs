using Binacle.Lib.Abstractions;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.AlgorithmProcessing;
using Binacle.TestsKernel.Models;

namespace Binacle.Lib.UnitTests;

[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class BinProcessingCancellationTests
{
	private static readonly IAlgorithmFactory algorithmFactory = new AlgorithmFactory();

	private static readonly TestOperationParameters packingParameters = new()
	{
		Operation = AlgorithmOperation.Packing
	};

	// Enough bins for a cancelled run to have somewhere to stop, and enough items that each bin costs real work.
	private static List<TestBin> CreateBins()
		=> Enumerable.Range(1, 20)
			.Select(index => new TestBin { ID = $"bin_{index}", Length = 100, Width = 100, Height = 100 })
			.ToList();

	private static List<TestItem> CreateItems()
		=> Enumerable.Range(1, 20)
			.Select(index => new TestItem { ID = $"item_{index}", Length = 10, Width = 10, Height = 10, Quantity = 10 })
			.ToList();

	[Fact(DisplayName = "LoopBinProcessor Throws When Token Is Already Cancelled")]
	public void LoopBinProcessor_Throws_When_Cancelled()
	{
		var processor = new LoopBinProcessor(algorithmFactory);
		using var cts = new CancellationTokenSource();
		cts.Cancel();

		Assert.Throws<OperationCanceledException>(() =>
			processor.Process(Algorithm.FFD, CreateBins(), CreateItems(), packingParameters, cts.Token)
		);
	}

	[Fact(DisplayName = "ParallelBinProcessor Throws When Token Is Already Cancelled")]
	public void ParallelBinProcessor_Throws_When_Cancelled()
	{
		var processor = new ParallelBinProcessor(algorithmFactory);
		using var cts = new CancellationTokenSource();
		cts.Cancel();

		Assert.Throws<OperationCanceledException>(() =>
			processor.Process(Algorithm.FFD, CreateBins(), CreateItems(), packingParameters, cts.Token)
		);
	}

	[Fact(DisplayName = "LoopAlgorithmProcessor Throws When Token Is Already Cancelled")]
	public void LoopAlgorithmProcessor_Throws_When_Cancelled()
	{
		var processor = new LoopAlgorithmProcessor([Algorithm.FFD, Algorithm.WFD, Algorithm.BFD], algorithmFactory);
		using var cts = new CancellationTokenSource();
		cts.Cancel();

		Assert.Throws<OperationCanceledException>(() =>
			processor.Process(CreateBins().First(), CreateItems(), packingParameters, cts.Token)
		);
	}

	[Fact(DisplayName = "Cancelling Mid-Run Stops Before Every Bin Is Processed")]
	public void Cancelling_MidRun_Stops_Early()
	{
		var bins = CreateBins();
		var processor = new LoopBinProcessor(new CountingAlgorithmFactory(algorithmFactory, out var counter));
		using var cts = new CancellationTokenSource();

		// Cancel once the run is underway, so the check is proven per bin rather than once on entry.
		counter.CancelAfter(cts, 3);

		Assert.Throws<OperationCanceledException>(() =>
			processor.Process(Algorithm.FFD, bins, CreateItems(), packingParameters, cts.Token)
		);
		Assert.True(counter.Count < bins.Count, $"expected to stop early, processed {counter.Count} of {bins.Count}");
	}

	private sealed class Counter
	{
		private CancellationTokenSource? source;
		private int cancelAt;
		public int Count { get; private set; }

		public void CancelAfter(CancellationTokenSource cts, int afterCount)
		{
			this.source = cts;
			this.cancelAt = afterCount;
		}

		public void Increment()
		{
			this.Count++;
			if (this.source is not null && this.Count == this.cancelAt)
			{
				this.source.Cancel();
			}
		}
	}

	private sealed class CountingAlgorithmFactory : IAlgorithmFactory
	{
		private readonly IAlgorithmFactory inner;
		private readonly Counter counter;

		public CountingAlgorithmFactory(IAlgorithmFactory inner, out Counter counter)
		{
			this.inner = inner;
			counter = new Counter();
			this.counter = counter;
		}

		public IPackingAlgorithm Create<TBin, TItem>(Algorithm algorithm, TBin bin, IList<TItem> items)
			where TBin : class, IWithID, IWithReadOnlyDimensions
			where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
		{
			this.counter.Increment();
			return this.inner.Create(algorithm, bin, items);
		}
	}
}
