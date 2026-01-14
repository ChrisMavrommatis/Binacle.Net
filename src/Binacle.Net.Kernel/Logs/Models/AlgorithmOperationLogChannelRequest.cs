using Binacle.Lib.Abstractions.Models;

namespace Binacle.Net.Kernel.Logs.Models;

public class AlgorithmOperationLogChannelRequest
{
	public required IReadOnlyCollection<IWithReadOnlyDimensions> Bins { get; init; }
	public required IReadOnlyCollection<IWithReadOnlyDimensions> Items { get; init; }
	public ILogConvertible? Parameters { get; set; }
	public required IDictionary<string, OperationResult> Results { get; init; }
	
	internal AlgorithmOperationLogChannelRequest() 
	{
	}
	
	public static AlgorithmOperationLogChannelRequest From<TBin, TItem, TParams>(
		List<TBin> bins,
		List<TItem> items,
		TParams parameters,
		IDictionary<string, OperationResult> results
	)
		where TBin: IWithID, IWithReadOnlyDimensions
		where TItem: IWithID, IWithReadOnlyDimensions, IWithQuantity
		where TParams: ILogConvertible
	{
		return new AlgorithmOperationLogChannelRequest()
		{
			Bins = bins.Cast<IWithDimensions>().ToList().AsReadOnly(),
			Items =	items.Cast<IWithDimensions>().ToList().AsReadOnly(),
			Parameters = parameters,
			Results = results,
		};
	}
}

