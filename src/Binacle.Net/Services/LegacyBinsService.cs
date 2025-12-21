using System.Threading.Channels;
using Binacle.Lib;
using Binacle.Lib.Abstractions;
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Fitting.Models;
using Binacle.Lib.Packing.Models;
using Binacle.Net.Kernel.Logs.Models;
using LibAlgorithm = Binacle.Lib.Algorithm;
using ApiPackingParameters = Binacle.Net.Models.PackingParameters;
using ApiFittingParameters = Binacle.Net.Models.FittingParameters;

namespace Binacle.Net.Services;

internal interface ILegacyBinsService
{
	ValueTask<IDictionary<string, FittingResult>> FitBinsAsync<TBin, TBox>(
		List<TBin> bins, 
		List<TBox> items,
		ApiFittingParameters parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity;

	ValueTask<IDictionary<string, PackingResult>> PackBinsAsync<TBin, TBox>(
		List<TBin> bins, 
		List<TBox> items,
		ApiPackingParameters parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity;
}

internal class LegacyBinsService : ILegacyBinsService
{
	private readonly IAlgorithmFactory algorithmFactory;
	private readonly Channel<FittingLogChannelRequest>? fittingChannel;
	private readonly Channel<PackingLogChannelRequest>? packingChannel;
	private readonly ILogger<LegacyBinsService> logger;

	public LegacyBinsService(
		IAlgorithmFactory algorithmFactory,
		IOptionalDependency<Channel<FittingLogChannelRequest>> fittingChannel,
		IOptionalDependency<Channel<PackingLogChannelRequest>> packingChannel,
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
		ApiFittingParameters parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		using var timedOperation = this.logger.BeginTimedOperation("Fit Bins");

		var results = new Dictionary<string, FittingResult>();

		foreach (var bin in bins.OrderBy(x => x.CalculateVolume()))
		{
			var algorithmInstance = this.algorithmFactory.CreateFitting(LibAlgorithm.FirstFitDecreasing, bin, items);
			var result = algorithmInstance.Execute(parameters);

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
		ApiFittingParameters parameters,
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
					FittingLogChannelRequest.From(bins, items, parameters, results)
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
		ApiPackingParameters parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		using var timedOperation = this.logger.BeginTimedOperation("Pack Bins");

		var results = new Dictionary<string, PackingResult>();

		foreach (var bin in bins.OrderBy(x => x.CalculateVolume()))
		{
			var algorithmInstance = this.algorithmFactory.CreatePacking(LibAlgorithm.FirstFitDecreasing, bin, items);
			var result = algorithmInstance.Execute(parameters);
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
		ApiPackingParameters parameters,
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
					PackingLogChannelRequest.From(bins, items, parameters, results)
				);
		}
		catch (Exception ex)
		{
			this.logger.LogError(ex, "Error while writing to channel");
		}
	}
}
