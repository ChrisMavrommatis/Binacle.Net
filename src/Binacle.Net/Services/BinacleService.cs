using System.Threading.Channels;
using Binacle.Lib;
using Binacle.Lib.Abstractions;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;
using Binacle.Net.ExtensionMethods;
using Binacle.Net.Kernel.Logs.Models;

namespace Binacle.Net.Services;

internal interface IBinacleService
{
	ValueTask<OperationResult> SingleBinAsync<TBin, TBox, TParams>(
		TBin bin,
		List<TBox> items,
		TParams parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
		where TParams : class, IOperationParameters, ILogConvertible;

	ValueTask<OperationResult> SingleBinAsync<TBin, TBox, TParams>(
		Algorithm algorithm,
		TBin bin,
		List<TBox> items,
		TParams parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
		where TParams : class, IOperationParameters, ILogConvertible;
	
	ValueTask<IDictionary<string, OperationResult>> CompareBinsAsync<TBin, TBox, TParams>(
		Algorithm algorithm,
		List<TBin> bins,
		List<TBox> items,
		TParams parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
		where TParams : class, IOperationParameters, ILogConvertible;
}

internal class BinacleService : IBinacleService
{
	private readonly Channel<AlgorithmOperationLogChannelRequest>? logChannel;
	private readonly IBinProcessorFactory binProcessorFactory;
	private readonly IAlgorithmProcessorFactory algorithmProcessorFactory;
	private readonly IResultSelector resultSelector;
	private readonly IAlgorithmFactory algorithmFactory;
	private readonly ILogger<BinacleService> logger;

	public BinacleService(
		IBinProcessorFactory binProcessorFactory,
		IAlgorithmProcessorFactory algorithmProcessorFactory,
		IResultSelector resultSelector,
		IAlgorithmFactory algorithmFactory,
		ILogger<BinacleService> logger,
		IOptionalDependency<Channel<AlgorithmOperationLogChannelRequest>> logChannel
	)
	{
		this.binProcessorFactory = binProcessorFactory;
		this.algorithmProcessorFactory = algorithmProcessorFactory;
		this.resultSelector = resultSelector;
		this.algorithmFactory = algorithmFactory;
		this.logChannel = logChannel.Value;
		this.logger = logger;
	}

	public async ValueTask<OperationResult> SingleBinAsync<TBin, TBox, TParams>(
		TBin bin,
		List<TBox> items,
		TParams parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
		where TParams : class, IOperationParameters, ILogConvertible
	{
		using var activity = Diagnostics.ActivitySource.StartActivity("Single Bin Operation");

		using var timedOperation = this.logger.BeginTimedOperation("Single Bin Operation");
		var algorithmProcessor = this.algorithmProcessorFactory.Create(items.Count);
		var results = algorithmProcessor.Process(
			bin,
			items,
			parameters
		);
		
		var result = resultSelector.BestAlgorithm(results);
		await this.logChannel.WriteToChannelAsync(
			bin,
			items,
			parameters,
			(result.AlgorithmInfo.GetAlgorithmIdentifierName(), result), 
			this.logger
		);
		return result;
	}
	
	public async ValueTask<OperationResult> SingleBinAsync<TBin, TBox, TParams>(
		Algorithm algorithm,
		TBin bin,
		List<TBox> items,
		TParams parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
		where TParams : class, IOperationParameters, ILogConvertible
	{
		using var activity = Diagnostics.ActivitySource.StartActivity("Single Bin Operation");

		using var timedOperation = this.logger.BeginTimedOperation("Single Bin Operation");
		var algorithmInstance = this.algorithmFactory.Create(algorithm, bin, items);
		var result = algorithmInstance.Execute(parameters);
		
		await this.logChannel.WriteToChannelAsync(
			bin,
			items,
			parameters,
			(result.AlgorithmInfo.GetAlgorithmIdentifierName(), result), 
			this.logger
		);
		return result;
	}

	public async ValueTask<IDictionary<string, OperationResult>> CompareBinsAsync<TBin, TBox, TParams>(
		Algorithm algorithm,
		List<TBin> bins,
		List<TBox> items,
		TParams parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
		where TParams : class, IOperationParameters, ILogConvertible
	{
		using var activity = Diagnostics.ActivitySource.StartActivity("Pack Bins");

		using var timedOperation = this.logger.BeginTimedOperation("Pack Bins");

		var binProcessor = this.binProcessorFactory.Create(bins.Count, items.Count);
		var results = binProcessor.Process(
			algorithm,
			bins,
			items,
			parameters
		);

		await this.logChannel.WriteToChannelAsync(bins, items, parameters, results, this.logger);
		return results;
	}
}
