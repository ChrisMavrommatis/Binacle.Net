using Binacle.Net.Api.ExtensionMethods;
using Binacle.Net.Lib;
using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Packing.Models;
using ChrisMavrommatis.Logging;
using PackingParameters = Binacle.Net.Api.Models.PackingParameters;

namespace Binacle.Net.Api.Services;

#pragma warning disable CS1591

public interface IBinacleService
{
	Dictionary<string, PackingResult> PackBins<TBin, TBox>(List<TBin> bins, List<TBox> items, PackingParameters parameters)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity;
}


internal class BinacleService : IBinacleService
{
	private readonly ILogger<BinacleService> logger;
	private readonly LoopBinProcessor loopBinProcessor;

	public BinacleService(
		ILogger<BinacleService> logger
	)
	{
		this.loopBinProcessor = new LoopBinProcessor();
		this.logger = logger;
	}
	
	public Dictionary<string, PackingResult> PackBins<TBin, TBox>(
		List<TBin> bins, 
		List<TBox> items,
		PackingParameters parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		using var timedOperation = this.logger.BeginTimedOperation("Pack Bins");

		this.logger.EnrichStateWith("Items", items);
		this.logger.EnrichStateWith("Bins", bins);
		this.logger.EnrichStateWithParameters(parameters);

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

		this.logger.EnrichStateWithResults(results);
		return results;
	}
}
