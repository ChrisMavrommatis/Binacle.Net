﻿using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Fitting.Models;
using Binacle.Net.Lib.Packing.Models;
using ChrisMavrommatis.Logging;

namespace Binacle.Net.Api.Services;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public interface ILockerService
{
	FittingResult FindFittingBin<TBin, TBox>(List<TBin> bins, List<TBox> items)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity;
	Dictionary<string, PackingResult> PackBins<TBin, TBox>(List<TBin> bins, List<TBox> items)
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


	public Dictionary<string, PackingResult> PackBins<TBin, TBox>(List<TBin> bins, List<TBox> items)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		using var timedOperation = this.logger.BeginTimedOperation("Pack Bins");

		timedOperation.WithNamedState("Items", items.ToDictionary(x => x.ID, x => $"{x.Height}x{x.Length}x{x.Width} q{x.Quantity}"));
		timedOperation.WithNamedState("Bins", bins.ToDictionary(x => x.ID, x => $"{x.Height}x{x.Length}x{x.Width}"));

		var results = new Dictionary<string, PackingResult>();

		foreach (var bin in bins)
		{
			var algorithmInstance = this.algorithmFactory.CreatePacking(Lib.Algorithm.FirstFitDecreasing, bin, items);
			var result = algorithmInstance.Execute();
			results.Add(bin.ID, result);
		}
		
		timedOperation.WithNamedState("Results", results);
		return results;
	}
	public FittingResult FindFittingBin<TBin, TBox>(List<TBin> bins, List<TBox> items)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		using var timedOperation = this.logger.BeginTimedOperation("Find Fitting Bin");

		timedOperation.WithNamedState("Items", items.ToDictionary(x => x.ID, x => $"{x.Height}x{x.Length}x{x.Width} q{x.Quantity}"));
		timedOperation.WithNamedState("Bins", bins.ToDictionary(x => x.ID, x => $"{x.Height}x{x.Length}x{x.Width}"));

		var strategy = this.algorithmFactory.Create(Lib.Algorithm.FirstFitDecreasing);

		var flatItems = items.SelectMany(x => Enumerable.Repeat(x, x.Quantity)).ToList();

		var operation = strategy
			.WithBins(bins)
			.AndItems(flatItems)
			.Build();

		var operationResult = operation.Execute();

		var resultState = new Dictionary<string, object>()
		{
			{ "Status", operationResult.Status.ToString() }
		};

		if (operationResult.FoundBin is not null)
		{
			var foundBin = new Dictionary<string, string>()
			{
				{operationResult.FoundBin.ID, $"{operationResult.FoundBin.Height}x{operationResult.FoundBin.Length}x{operationResult.FoundBin.Width}" }
			};
			resultState.Add("FoundBin", foundBin);
		}
		
		if(operationResult.Reason.HasValue) 
		{
			resultState.Add("Reason", operationResult.Reason.Value.ToString());
		}

		timedOperation.WithNamedState("Result", resultState);
		return operationResult;
	}
}
