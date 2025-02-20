using System.Threading.Channels;
using Binacle.Net.Api.Kernel.Models;
using Binacle.Net.Lib;
using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Packing.Models;
using ChrisMavrommatis.Logging;
using PackingParameters = Binacle.Net.Api.Models.PackingParameters;

namespace Binacle.Net.Api.Services;

#pragma warning disable CS1591

public interface IBinacleService
{
	Task<Dictionary<string, PackingResult>> PackBinsAsync<TBin, TBox>(List<TBin> bins, List<TBox> items, PackingParameters parameters)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity;
}


internal class BinacleService : IBinacleService
{
	private readonly Channel<PackingLogChannelRequest>? packingChannel;
	private readonly ILogger<BinacleService> logger;
	private readonly LoopBinProcessor loopBinProcessor;

	public BinacleService(
		ILogger<BinacleService> logger,
		IOptionalDependency<Channel<PackingLogChannelRequest>> packingChannel
	)
	{
		this.loopBinProcessor = new LoopBinProcessor();
		this.packingChannel = packingChannel.Value;
		this.logger = logger;
	}
	
	public async Task<Dictionary<string, PackingResult>> PackBinsAsync<TBin, TBox>(
		List<TBin> bins, 
		List<TBox> items,
		PackingParameters parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		using var timedOperation = this.logger.BeginTimedOperation("Pack Bins");

		var results = this.loopBinProcessor.ProcessPacking(
			parameters.GetMappedAlgorithm(),
			bins,
			items,
			new Lib.Packing.Models.PackingParameters 
			{ 
				NeverReportUnpackedItems = false, 
				OptInToEarlyFails = false,
				ReportPackedItemsOnlyWhenFullyPacked = false
			});

		if (this.packingChannel is not null)
		{
			await this.packingChannel
				.Writer
				.WriteAsync(
					PackingLogChannelRequest.From(bins, items, parameters, results)
				);
		}
		
		return results;
	}
}
