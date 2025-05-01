using System.Threading.Channels;
using Binacle.Net.Kernel.Logs.Models;
using Binacle.Lib.Abstractions;
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Packing.Models;
using ChrisMavrommatis.Logging;
using ApiPackingParameters = Binacle.Net.Models.PackingParameters;
using LibPackingParameters = Binacle.Lib.Packing.Models.PackingParameters;

namespace Binacle.Net.Services;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public interface IBinacleService
{
	ValueTask<IDictionary<string, PackingResult>> PackBinsAsync<TBin, TBox>(
		List<TBin> bins,
		List<TBox> items,
		ApiPackingParameters parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity;
}

internal class BinacleService : IBinacleService
{
	private readonly Channel<PackingLogChannelRequest>? packingChannel;
	private readonly ILogger<BinacleService> logger;
	private readonly IBinProcessor loopBinProcessor;
	private readonly IBinProcessor parallelBinProcessor;

	public BinacleService(
		[FromKeyedServices("loop")] IBinProcessor loopBinProcessor,
		[FromKeyedServices("parallel")] IBinProcessor parallelBinProcessor,
		ILogger<BinacleService> logger,
		IOptionalDependency<Channel<PackingLogChannelRequest>> packingChannel
	)
	{
		this.loopBinProcessor = loopBinProcessor;
		this.parallelBinProcessor = parallelBinProcessor;
		this.packingChannel = packingChannel.Value;
		this.logger = logger;
	}

	public async ValueTask<IDictionary<string, PackingResult>> PackBinsAsync<TBin, TBox>(
		List<TBin> bins,
		List<TBox> items,
		ApiPackingParameters parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		using var activity = Diagnostics.ActivitySource.StartActivity("Pack Bins");

		using var timedOperation = this.logger.BeginTimedOperation("Pack Bins");

		var results = this.loopBinProcessor.ProcessPacking(
			parameters.GetMappedAlgorithm(),
			bins,
			items,
			new LibPackingParameters
			{
				NeverReportUnpackedItems = false,
				OptInToEarlyFails = false,
				ReportPackedItemsOnlyWhenFullyPacked = false
			});

		await this.WriteToChannelAsync(bins, items, parameters, results);
		return results;
	}

	private async ValueTask WriteToChannelAsync<TBin, TBox>(
		List<TBin> bins,
		List<TBox> items,
		ApiPackingParameters parameters,
		IDictionary<string, PackingResult> results
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		using var channelActivity = Diagnostics.ActivitySource.StartActivity("Send Channel Request");

		if (this.packingChannel is null)
		{
			return;
		}

		try
		{
			await this.packingChannel
				.Writer
				.WriteAsync(
					PackingLogChannelRequest.From(bins, items, parameters, results)
				);
		}
		catch (Exception ex)
		{
			this.logger.LogError(ex, "Error while writing to channel");
		}
	}
}
