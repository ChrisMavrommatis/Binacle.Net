using Binacle.Net.Lib;
using Binacle.Net.Lib.Abstractions.Models;
using ChrisMavrommatis.Logging;

namespace Binacle.Net.Api.Services;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public interface ILockerService
{
	Dictionary<string, Lib.Fitting.Models.FittingResult> FitBins<TBin, TBox>(List<TBin> bins, List<TBox> items, Models.FittingParameters parameters)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity;

	Dictionary<string, Lib.Packing.Models.PackingResult> PackBins<TBin, TBox>(List<TBin> bins, List<TBox> items, Models.PackingParameters parameters)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity;
}

internal class LockerService : ILockerService
{
	private readonly Lib.AlgorithmFactory algorithmFactory;
	private readonly ILogger<LockerService> logger;

	public LockerService(ILogger<LockerService> logger)
	{
		this.algorithmFactory = new Lib.AlgorithmFactory();
		this.logger = logger;
	}

	public Dictionary<string, Lib.Fitting.Models.FittingResult> FitBins<TBin, TBox>(
		List<TBin> bins,
		List<TBox> items,
		Models.FittingParameters parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		using var timedOperation = this.logger.BeginTimedOperation("Fit Bins");

		timedOperation.WithNamedState("Items", items.ToDictionary(x => x.ID, x => $"{x.Height}x{x.Length}x{x.Width} q{x.Quantity}"));
		timedOperation.WithNamedState("Bins", bins.ToDictionary(x => x.ID, x => $"{x.Height}x{x.Length}x{x.Width}"));

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

		timedOperation.WithNamedState("Results", results);
		return results;


		// TODO Fix logging
		//var resultState = new Dictionary<string, object>()
		//{
		//	{ "Status", operationResult.Status.ToString() }
		//};

		//if (operationResult.FoundBin is not null)
		//{
		//	var foundBin = new Dictionary<string, string>()
		//	{
		//		{operationResult.FoundBin.ID, $"{operationResult.FoundBin.Height}x{operationResult.FoundBin.Length}x{operationResult.FoundBin.Width}" }
		//	};
		//	resultState.Add("FoundBin", foundBin);
		//}

		//if(operationResult.Reason.HasValue) 
		//{
		//	resultState.Add("Reason", operationResult.Reason.Value.ToString());
		//}

		//timedOperation.WithNamedState("Result", resultState);
		//return operationResult;
	}

	public Dictionary<string, Lib.Packing.Models.PackingResult> PackBins<TBin, TBox>(
		List<TBin> bins, 
		List<TBox> items,
		Models.PackingParameters parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		using var timedOperation = this.logger.BeginTimedOperation("Pack Bins");

		timedOperation.WithNamedState("Items", items.ToDictionary(x => x.ID, x => $"{x.Height}x{x.Length}x{x.Width} q{x.Quantity}"));
		timedOperation.WithNamedState("Bins", bins.ToDictionary(x => x.ID, x => $"{x.Height}x{x.Length}x{x.Width}"));

		var results = new Dictionary<string, Lib.Packing.Models.PackingResult>();

		foreach (var bin in bins)
		{
			var algorithmInstance = this.algorithmFactory.CreatePacking(Lib.Algorithm.FirstFitDecreasing, bin, items);
			var result = algorithmInstance.Execute(new Lib.Packing.Models.PackingParameters 
			{ 
				NeverReportUnpackedItems = parameters.NeverReportUnpackedItems, 
				OptInToEarlyFails = parameters.OptInToEarlyFails,
				ReportPackedItemsOnlyWhenFullyPacked = parameters.ReportPackedItemsOnlyWhenFullyPacked
			});
			results.Add(bin.ID, result);
		}
		
		timedOperation.WithNamedState("Results", results);
		return results;
	}
	
}
