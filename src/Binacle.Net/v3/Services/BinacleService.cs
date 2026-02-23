using System.Threading.Channels;
using Binacle.Lib.Abstractions;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;
using Binacle.Net.ExtensionMethods;
using Binacle.Net.Kernel.Logs.Models;
using Binacle.Net.Services;
using Binacle.Net.v3.Contracts;
using Binacle.Net.v3.ExtensionMethods;

namespace Binacle.Net.v3.Services;

internal interface IBinacleService
{
	ValueTask<IDictionary<string, OperationResult>> OperateAsync<TBin, TBox, TParams>(
		List<TBin> bins,
		List<TBox> items,
		TParams parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
		where TParams : class, IWithAlgorithm, IOperationParameters, ILogConvertible;
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

	public async ValueTask<IDictionary<string, OperationResult>> OperateAsync<TBin, TBox, TParams>(
		List<TBin> bins,
		List<TBox> items,
		TParams parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
		where TParams : class, IWithAlgorithm, IOperationParameters, ILogConvertible
	{
		using var activity = Diagnostics.ActivitySource.StartActivity("Pack Bins");

		using var timedOperation = this.logger.BeginTimedOperation("Pack Bins");

		var results = this.loopBinProcessor.Process(
			parameters.Algorithm.ToLibAlgorithm(),
			bins,
			items,
			parameters
		);

		await this.logChannel.WriteToChannelAsync(bins, items, parameters, results, this.logger);
		return results;
	}
}
