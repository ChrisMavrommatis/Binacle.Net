using System.Threading.Channels;
using Binacle.Lib.Abstractions;
using Binacle.Lib.Abstractions.Models;
using Binacle.Net.ExtensionMethods;
using Binacle.Net.Kernel.Logs.Models;
using ApiOperationParameters = Binacle.Net.Models.OperationParameters;

namespace Binacle.Net.Services;

internal interface IBinacleService
{
	ValueTask<IDictionary<string, OperationResult>> OperateAsync<TBin, TBox>(
		List<TBin> bins,
		List<TBox> items,
		ApiOperationParameters parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity;
}

internal class BinacleService : IBinacleService
{
	private readonly Channel<AlgorithmOperationLogChannelRequest>? logChannel;
	private readonly ILogger<BinacleService> logger;
	private readonly IBinProcessor loopBinProcessor;
	private readonly IBinProcessor parallelBinProcessor;

	public BinacleService(
		[FromKeyedServices("loop")] IBinProcessor loopBinProcessor,
		[FromKeyedServices("parallel")] IBinProcessor parallelBinProcessor,
		ILogger<BinacleService> logger,
		IOptionalDependency<Channel<AlgorithmOperationLogChannelRequest>> logChannel
	)
	{
		this.loopBinProcessor = loopBinProcessor;
		this.parallelBinProcessor = parallelBinProcessor;
		this.logChannel = logChannel.Value;
		this.logger = logger;
	}

	public async ValueTask<IDictionary<string, OperationResult>> OperateAsync<TBin, TBox>(
		List<TBin> bins,
		List<TBox> items,
		ApiOperationParameters parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		using var activity = Diagnostics.ActivitySource.StartActivity("Pack Bins");

		using var timedOperation = this.logger.BeginTimedOperation("Pack Bins");

		var results = this.loopBinProcessor.Process(
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
		ApiOperationParameters parameters,
		IDictionary<string, OperationResult> results
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		using var channelActivity = Diagnostics.ActivitySource.StartActivity("Send Channel Request");

		if (this.logChannel is null)
		{
			return;
		}

		try
		{
			await this.logChannel
				.Writer
				.WriteAsync(
					AlgorithmOperationLogChannelRequest.From(bins, items, parameters, results)
				);
		}
		catch (Exception ex)
		{
			this.logger.LogError(ex, "Error while writing to channel");
		}
	}
}
