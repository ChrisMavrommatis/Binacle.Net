using System.Threading.Channels;
using Binacle.Lib;
using Binacle.Lib.Abstractions;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;
using Binacle.Net.ExtensionMethods;
using Binacle.Net.Kernel.Logs.Models;
using Binacle.Net.DiagnosticsModule.Logs.Models;

namespace Binacle.Net.Services;

internal interface IBinacleService
{
	ValueTask<OperationResult> SingleBinAsync<TBin, TBox, TParams>(
		Algorithm algorithm,
		TBin bin,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider;
	
	ValueTask<OperationResult> SingleBinAsync<TBin, TBox, TParams>(
		TBin bin,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider;

	
	ValueTask<IDictionary<string, OperationResult>> MultipleBinsAsync<TBin, TBox, TParams>(
		Algorithm algorithm,
		List<TBin> bins,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider;
	
	ValueTask<IDictionary<string, OperationResult>> MultipleBinsAsync<TBin, TBox, TParams>(
		List<TBin> bins,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider;
	
	ValueTask<OperationResult> SmallestBinAsync<TBin, TBox, TParams>(
		Algorithm algorithm,
		List<TBin> bins,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider;
	
	ValueTask<OperationResult> SmallestBinAsync<TBin, TBox, TParams>(
		List<TBin> bins,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider;

	ValueTask<OperationResult> BestBinAsync<TBin, TBox, TParams>(
		Algorithm algorithm,
		List<TBin> bins,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider;

	ValueTask<OperationResult> BestBinAsync<TBin, TBox, TParams>(
		List<TBin> bins,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider;
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
		Algorithm algorithm,
		TBin bin,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider
	{
		using var timedOperation = this.logger.BeginTimedActivityOperation("Single Bin Operation");
		var algorithmInstance = this.algorithmFactory.Create(algorithm, bin, items);
		var result = algorithmInstance.Execute(parameters);
		
		await this.logChannel.WriteToChannelAsync(
			[bin],
			items,
			parameters,
			(result.AlgorithmInfo.GetAlgorithmIdentifierName(), result),
			this.logger
		);
		return result;
	}
	
	public async ValueTask<OperationResult> SingleBinAsync<TBin, TBox, TParams>(
		TBin bin,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider
	{
		using var timedOperation = this.logger.BeginTimedActivityOperation("Single Bin Operation");
		var algorithmProcessor = this.algorithmProcessorFactory.Create(items.Count);
		var results = algorithmProcessor.Process(
			bin,
			items,
			parameters,
			cancellationToken
		);
		
		var result = this.resultSelector.BestAlgorithm(results);
		await this.logChannel.WriteToChannelAsync(
			[bin],
			items,
			parameters,
			(result.AlgorithmInfo.GetAlgorithmIdentifierName(), result),
			this.logger
		);
		return result;
	}
	
	

	public async ValueTask<IDictionary<string, OperationResult>> MultipleBinsAsync<TBin, TBox, TParams>(
		Algorithm algorithm,
		List<TBin> bins,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider
	{
		using var timedOperation = this.logger.BeginTimedActivityOperation("Multiple Bin Operation");

		var binProcessor = this.binProcessorFactory.Create(bins.Count, items.Count);
		var results = binProcessor.Process(
			algorithm,
			bins,
			items,
			parameters,
			cancellationToken
		);

		await this.logChannel.WriteToChannelAsync(bins, items, parameters, results, this.logger);
		return results;
	}

	public async ValueTask<IDictionary<string, OperationResult>> MultipleBinsAsync<TBin, TBox, TParams>(
		List<TBin> bins,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider
	{
		using var timedOperation = this.logger.BeginTimedActivityOperation("Multiple Bin Operation");

		var binProcessor = this.binProcessorFactory.CreateMultiAlgorithm(bins.Count, items.Count);
		var results = binProcessor.Process(
			bins,
			items,
			parameters,
			cancellationToken
		);

		await this.logChannel.WriteToChannelAsync(bins, items, parameters, results, this.logger);
		return results;
	}
	
	public async ValueTask<OperationResult> SmallestBinAsync<TBin, TBox, TParams>(
		Algorithm algorithm,
		List<TBin> bins,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider
	{
		using var timedOperation = this.logger.BeginTimedActivityOperation("Smallest Bin Operation");

		var binProcessor = this.binProcessorFactory.Create(bins.Count, items.Count);
		var results = binProcessor.Process(
			algorithm,
			bins,
			items,
			parameters,
			cancellationToken
		);
		var result = this.resultSelector.SmallestBin(results);

		await this.logChannel.WriteToChannelAsync(
			bins, 
			items,
			parameters,
			(result.Bin.ID, result),
			this.logger
		);
		return result;
	}

	public async ValueTask<OperationResult> SmallestBinAsync<TBin, TBox, TParams>(
		List<TBin> bins,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider
	{
		using var timedOperation = this.logger.BeginTimedActivityOperation("Smallest Bin Operation");

		var binProcessor = this.binProcessorFactory.CreateMultiAlgorithm(bins.Count, items.Count);
		var results = binProcessor.Process(
			bins,
			items,
			parameters,
			cancellationToken
		);
		var result = this.resultSelector.SmallestBin(results);

		await this.logChannel.WriteToChannelAsync(
			bins,
			items,
			parameters,
			(result.Bin.ID, result),
			this.logger
		);
		return result;
	}

	public async ValueTask<OperationResult> BestBinAsync<TBin, TBox, TParams>(
		Algorithm algorithm,
		List<TBin> bins,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider
	{
		using var timedOperation = this.logger.BeginTimedActivityOperation("Best Bin Operation");

		var binProcessor = this.binProcessorFactory.Create(bins.Count, items.Count);
		var results = binProcessor.Process(
			algorithm,
			bins,
			items,
			parameters,
			cancellationToken
		);
		var result = this.resultSelector.BestBin(results);

		await this.logChannel.WriteToChannelAsync(
			bins,
			items,
			parameters,
			(result.Bin.ID, result),
			this.logger
		);
		return result;
	}

	public async ValueTask<OperationResult> BestBinAsync<TBin, TBox, TParams>(
		List<TBin> bins,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider
	{
		using var timedOperation = this.logger.BeginTimedActivityOperation("Best Bin Operation");

		var binProcessor = this.binProcessorFactory.CreateMultiAlgorithm(bins.Count, items.Count);
		var results = binProcessor.Process(
			bins,
			items,
			parameters,
			cancellationToken
		);
		var result = this.resultSelector.BestBin(results);

		await this.logChannel.WriteToChannelAsync(
			bins,
			items,
			parameters,
			(result.Bin.ID, result),
			this.logger
		);
		return result;
	}
}
