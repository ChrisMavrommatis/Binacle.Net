using System.Threading.Channels;
using Binacle.Net.Kernel.Logs.Models;
using Binacle.Lib.Abstractions;
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Fitting.Models;
using Binacle.Lib.Packing.Models;
using Binacle.Net.ExtensionMethods;
using ApiPackingParameters = Binacle.Net.Models.PackingParameters;
using ApiFittingParameters = Binacle.Net.Models.FittingParameters;

namespace Binacle.Net.Services;

internal interface IBinacleService
{
	ValueTask<IDictionary<string, PackingResult>> PackBinsAsync<TBin, TBox>(
		List<TBin> bins,
		List<TBox> items,
		ApiPackingParameters parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity;
	
	ValueTask<IDictionary<string, FittingResult>> FitBinsAsync<TBin, TBox>(
		List<TBin> bins,
		List<TBox> items,
		ApiFittingParameters parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity;
}

internal class BinacleService : IBinacleService
{
	private readonly Channel<PackingLogChannelRequest>? packingChannel;
	private readonly Channel<FittingLogChannelRequest>? fittingChannel;
	private readonly ILogger<BinacleService> logger;
	private readonly IBinProcessor loopBinProcessor;
	private readonly IBinProcessor parallelBinProcessor;

	public BinacleService(
		[FromKeyedServices("loop")] IBinProcessor loopBinProcessor,
		[FromKeyedServices("parallel")] IBinProcessor parallelBinProcessor,
		ILogger<BinacleService> logger,
		IOptionalDependency<Channel<PackingLogChannelRequest>> packingChannel,
		IOptionalDependency<Channel<FittingLogChannelRequest>> fittingChannel
	)
	{
		this.loopBinProcessor = loopBinProcessor;
		this.parallelBinProcessor = parallelBinProcessor;
		this.packingChannel = packingChannel.Value;
		this.logger = logger;
		this.fittingChannel = fittingChannel.Value;
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
			parameters.Algorithm.ToLibAlgorithm(),
			bins,
			items,
			parameters
		);

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
	public async ValueTask<IDictionary<string, FittingResult>> FitBinsAsync<TBin, TBox>(
		List<TBin> bins,
		List<TBox> items,
		ApiFittingParameters parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		using var activity = Diagnostics.ActivitySource.StartActivity("Fit Bins");

		using var timedOperation = this.logger.BeginTimedOperation("Fit Bins");

		var results = this.loopBinProcessor.ProcessFitting(
			parameters.Algorithm.ToLibAlgorithm(),
			bins,
			items,
			parameters
		);

		await this.WriteToChannelAsync(bins, items, parameters, results);
		return results;
	}

	private async Task WriteToChannelAsync<TBin, TBox>(
		List<TBin> bins,
		List<TBox> items,
		ApiFittingParameters parameters, 
		IDictionary<string, FittingResult> results
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions 
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		using var channelActivity = Diagnostics.ActivitySource.StartActivity("Send Channel Request");

		if (this.fittingChannel is null)
		{
			return;
		}

		try
		{
			await this.fittingChannel
				.Writer
				.WriteAsync(
					FittingLogChannelRequest.From(bins, items, parameters, results)
				);
		}
		catch (Exception ex)
		{
			this.logger.LogError(ex, "Error while writing to channel");
		}
	}
}
