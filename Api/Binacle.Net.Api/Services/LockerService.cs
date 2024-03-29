﻿using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Models;

namespace Binacle.Net.Api.Services;

public interface ILockerService
{
	public BinFittingOperationResult FindFittingBin<TBin, TBox>(List<TBin> bins, List<TBox> items)
		where TBin : class, IItemWithReadOnlyDimensions<int>
		where TBox : class, IItemWithReadOnlyDimensions<int>, IWithQuantity<int>;

}

public class LockerService : ILockerService
{
	private readonly Lib.StrategyFactory strategyFactory;

	public LockerService()
	{
		this.strategyFactory = new Lib.StrategyFactory();
	}

	public BinFittingOperationResult FindFittingBin<TBin, TBox>(List<TBin> bins, List<TBox> items)
		where TBin : class, IItemWithReadOnlyDimensions<int>
		where TBox : class, IItemWithReadOnlyDimensions<int>, IWithQuantity<int>
	{
		var strategy = this.strategyFactory.Create(Lib.BinFittingStrategy.FirstFitDecreasing);

		var operation = strategy
			.WithBins(bins)
			.AndItems(items)
			.Build();

		var operationResult = operation.Execute();
		return operationResult;
	}
}
