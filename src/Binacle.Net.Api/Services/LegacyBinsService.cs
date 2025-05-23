﻿using System.Threading.Channels;
using Binacle.Net.Api.Kernel.Models;
using Binacle.Net.Api.Models;
using Binacle.Net.Lib;
using Binacle.Net.Lib.Abstractions;
using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Fitting.Models;
using Binacle.Net.Lib.Packing.Models;
using ChrisMavrommatis.Logging;
using LibAlgorithm = Binacle.Net.Lib.Algorithm;
using LibPackingParameters = Binacle.Net.Lib.Packing.Models.PackingParameters;

namespace Binacle.Net.Api.Services;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public interface ILegacyBinsService
{
	ValueTask<IDictionary<string, FittingResult>> FitBinsAsync<TBin, TBox>(
		List<TBin> bins, 
		List<TBox> items,
		LegacyFittingParameters parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity;

	ValueTask<IDictionary<string, PackingResult>> PackBinsAsync<TBin, TBox>(
		List<TBin> bins, 
		List<TBox> items,
		LegacyPackingParameters parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity;
}

internal class LegacyBinsService : ILegacyBinsService
{
	private readonly IAlgorithmFactory algorithmFactory;
	private readonly Channel<LegacyFittingLogChannelRequest>? fittingChannel;
	private readonly Channel<LegacyPackingLogChannelRequest>? packingChannel;
	private readonly ILogger<LegacyBinsService> logger;

	public LegacyBinsService(
		IAlgorithmFactory algorithmFactory,
		IOptionalDependency<Channel<LegacyFittingLogChannelRequest>> fittingChannel,
		IOptionalDependency<Channel<LegacyPackingLogChannelRequest>> packingChannel,
		ILogger<LegacyBinsService> logger
	)
	{
		this.algorithmFactory = algorithmFactory;
		this.fittingChannel = fittingChannel.Value;
		this.packingChannel = packingChannel.Value;
		this.logger = logger;
	}

	public async ValueTask<IDictionary<string, FittingResult>> FitBinsAsync<TBin, TBox>(
		List<TBin> bins,
		List<TBox> items,
		LegacyFittingParameters parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		using var timedOperation = this.logger.BeginTimedOperation("Fit Bins");

		var results = new Dictionary<string, FittingResult>();

		foreach (var bin in bins.OrderBy(x => x.CalculateVolume()))
		{
			var algorithmInstance = this.algorithmFactory.CreateFitting(LibAlgorithm.FirstFitDecreasing, bin, items);
			var result = algorithmInstance.Execute(new FittingParameters
			{
				ReportFittedItems = parameters.ReportFittedItems,
				ReportUnfittedItems = parameters.ReportUnfittedItems
			});

			if (parameters.FindSmallestBinOnly)
			{
				if (result.Status == FittingResultStatus.Success)
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

		await this.WriteToChannelAsync(bins, items, parameters, results);
		return results;
	}

	private async ValueTask WriteToChannelAsync<TBin, TBox>(
		List<TBin> bins,
		List<TBox> items,
		LegacyFittingParameters parameters,
		Dictionary<string, FittingResult> results
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
					LegacyFittingLogChannelRequest.From(bins, items, parameters, results)
				);
		}
		catch (Exception ex)
		{
			this.logger.LogError(ex, "Error while writing to channel");
		}
	}

	public async ValueTask<IDictionary<string, PackingResult>> PackBinsAsync<TBin, TBox>(
		List<TBin> bins,
		List<TBox> items,
		LegacyPackingParameters parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		using var timedOperation = this.logger.BeginTimedOperation("Pack Bins");

		var results = new Dictionary<string, PackingResult>();

		foreach (var bin in bins.OrderBy(x => x.CalculateVolume()))
		{
			var algorithmInstance = this.algorithmFactory.CreatePacking(LibAlgorithm.FirstFitDecreasing, bin, items);
			var result = algorithmInstance.Execute(new LibPackingParameters
			{
				NeverReportUnpackedItems = parameters.NeverReportUnpackedItems,
				OptInToEarlyFails = parameters.OptInToEarlyFails,
				ReportPackedItemsOnlyWhenFullyPacked = parameters.ReportPackedItemsOnlyWhenFullyPacked
			});
			if (parameters.StopAtSmallestBin)
			{
				if (result.Status == PackingResultStatus.FullyPacked)
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

		await this.WriteToChannelAsync(bins, items, parameters, results);
		return results;
	}

	private async ValueTask WriteToChannelAsync<TBin, TBox>(
		List<TBin> bins,
		List<TBox> items,
		LegacyPackingParameters parameters,
		Dictionary<string, PackingResult> results
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
					LegacyPackingLogChannelRequest.From(bins, items, parameters, results)
				);
		}
		catch (Exception ex)
		{
			this.logger.LogError(ex, "Error while writing to channel");
		}
	}
}
