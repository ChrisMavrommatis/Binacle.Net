using System.Threading.Channels;
using Binacle.Lib.Abstractions;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;
using Binacle.Net.ExtensionMethods;
using Binacle.Net.Kernel.Logs.Models;
using Binacle.Net.Models;

namespace Binacle.Net.Services;

internal interface IBinacleService
{
	ValueTask<IDictionary<string, OperationResult>> OperateAsync<TBin, TBox, TParams>(
		List<TBin> bins,
		List<TBox> items,
		TParams parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
		where TParams : class, ILibAlgorithmConvertible, IOperationParameters, ILogConvertible;
}

internal class BinacleService : IBinacleService
{
	private readonly Channel<AlgorithmOperationLogChannelRequest>? logChannel;
	private readonly IBinProcessorFactory binProcessorFactory;
	private readonly ILogger<BinacleService> logger;

	public BinacleService(
		IBinProcessorFactory binProcessorFactory,
		ILogger<BinacleService> logger,
		IOptionalDependency<Channel<AlgorithmOperationLogChannelRequest>> logChannel
	)
	{
		this.binProcessorFactory = binProcessorFactory;
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
		where TParams : class, ILibAlgorithmConvertible, IOperationParameters, ILogConvertible
	{
		using var activity = Diagnostics.ActivitySource.StartActivity("Pack Bins");

		using var timedOperation = this.logger.BeginTimedOperation("Pack Bins");

		var binProcessor = this.binProcessorFactory.Create(bins.Count, items.Count);
		var results = binProcessor.Process(
			parameters.GetAlgorithm(),
			bins,
			items,
			parameters
		);

		await this.logChannel.WriteToChannelAsync(bins, items, parameters, results, this.logger);
		return results;
	}
	
	
}
