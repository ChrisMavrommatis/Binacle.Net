using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Models;
using ChrisMavrommatis.Logging.ExtensionMethods;

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
	private readonly ILogger<LockerService> logger;

	public LockerService(ILogger<LockerService> logger)
	{
		this.strategyFactory = new Lib.StrategyFactory();
		this.logger = logger;
	}

	public BinFittingOperationResult FindFittingBin<TBin, TBox>(List<TBin> bins, List<TBox> items)
		where TBin : class, IItemWithReadOnlyDimensions<int>
		where TBox : class, IItemWithReadOnlyDimensions<int>, IWithQuantity<int>
	{
		using var timedOperation = this.logger.BeginTimedOperation("Find Fitting Bin");

		timedOperation.WithNamedState("Items", items.ToDictionary(x => x.ID, x => $"{x.Height}x{x.Length}x{x.Width} q{x.Quantity}"));
		timedOperation.WithNamedState("Bins", bins.ToDictionary(x => x.ID, x => $"{x.Height}x{x.Length}x{x.Width}"));

		var strategy = this.strategyFactory.Create(Lib.BinFittingStrategy.FirstFitDecreasing);

		var operation = strategy
			.WithBins(bins)
			.AndItems(items)
			.Build();

		var operationResult = operation.Execute();

		var resultState = new Dictionary<string, object>()
		{
			{ "Status", operationResult.Status.ToString() }
		};

		if (operationResult.FoundBin is not null)
		{
			resultState.Add("FoundBin", $"{operationResult.FoundBin.Height}x{operationResult.FoundBin.Length}x{operationResult.FoundBin.Width}");
		}

		timedOperation.WithNamedState("Result", resultState);
		return operationResult;
	}
}
