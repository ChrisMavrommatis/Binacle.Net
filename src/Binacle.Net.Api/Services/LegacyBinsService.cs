﻿using Binacle.Net.Api.ExtensionMethods;
using Binacle.Net.Lib;
using Binacle.Net.Lib.Abstractions.Models;
using ChrisMavrommatis.Logging;

namespace Binacle.Net.Api.Services;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public interface ILegacyBinsService
{
	Dictionary<string, Lib.Fitting.Models.FittingResult> FitBins<TBin, TBox>(List<TBin> bins, List<TBox> items, Models.LegacyFittingParameters parameters)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity;

	Dictionary<string, Lib.Packing.Models.PackingResult> PackBins<TBin, TBox>(List<TBin> bins, List<TBox> items, Models.LegacyPackingParameters parameters)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity;
}

internal class LegacyBinsService : ILegacyBinsService
{
	private readonly Lib.AlgorithmFactory algorithmFactory;
	private readonly ILogger<LegacyBinsService> logger;

	public LegacyBinsService(ILogger<LegacyBinsService> logger)
	{
		this.algorithmFactory = new Lib.AlgorithmFactory();
		this.logger = logger;
	}

	public Dictionary<string, Lib.Fitting.Models.FittingResult> FitBins<TBin, TBox>(
		List<TBin> bins,
		List<TBox> items,
		Models.LegacyFittingParameters parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		using var timedOperation = this.logger.BeginTimedOperation("Fit Bins");
		this.logger.EnrichStateWith("Items", items);
		this.logger.EnrichStateWith("Bins", bins);
		this.logger.EnrichStateWithParameters(parameters);

		var results = new Dictionary<string, Lib.Fitting.Models.FittingResult>();

		foreach (var bin in bins.OrderBy(x => x.CalculateVolume()))
		{
			var algorithmInstance = this.algorithmFactory.CreateFitting(Lib.Algorithm.FirstFitDecreasing, bin, items);
			var result = algorithmInstance.Execute(new Lib.Fitting.Models.FittingParameters
			{
				ReportFittedItems = parameters.ReportFittedItems,
				ReportUnfittedItems = parameters.ReportUnfittedItems
			});

			if(parameters.FindSmallestBinOnly)
			{
				if(result.Status == Lib.Fitting.Models.FittingResultStatus.Success)
				{
					results.Add(bin.ID, result);
					break;
				}
				
			}
			else
			{
				results.Add(bin.ID, result);
			}
		}

		this.logger.EnrichStateWithResults(results);
		return results;
	}

	public Dictionary<string, Lib.Packing.Models.PackingResult> PackBins<TBin, TBox>(
		List<TBin> bins, 
		List<TBox> items,
		Models.LegacyPackingParameters parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		using var timedOperation = this.logger.BeginTimedOperation("Pack Bins");

		this.logger.EnrichStateWith("Items", items);
		this.logger.EnrichStateWith("Bins", bins);
		this.logger.EnrichStateWithParameters(parameters);

		var results = new Dictionary<string, Lib.Packing.Models.PackingResult>();

		foreach (var bin in bins.OrderBy(x => x.CalculateVolume()))
		{
			var algorithmInstance = this.algorithmFactory.CreatePacking(Algorithm.FirstFitDecreasing, bin, items);
			var result = algorithmInstance.Execute(new Lib.Packing.Models.PackingParameters 
			{ 
				NeverReportUnpackedItems = parameters.NeverReportUnpackedItems, 
				OptInToEarlyFails = parameters.OptInToEarlyFails,
				ReportPackedItemsOnlyWhenFullyPacked = parameters.ReportPackedItemsOnlyWhenFullyPacked
			});
			if (parameters.StopAtSmallestBin)
			{
				if (result.Status == Lib.Packing.Models.PackingResultStatus.FullyPacked)
				{
					results.Add(bin.ID, result);
					break;
				}

			}
			else
			{
				results.Add(bin.ID, result);
			}
		}

		this.logger.EnrichStateWithResults(results);
		return results;
	}
}
