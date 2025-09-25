using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Fitting.Models;

namespace Binacle.Net.Kernel.Logs.Models;

public class FittingLogChannelRequest
{
	public required IReadOnlyCollection<IWithReadOnlyDimensions> Bins { get; init; }
	public required IReadOnlyCollection<IWithReadOnlyDimensions> Items { get; init; }
	public ILogConvertible? Parameters { get; set; }
	public required IDictionary<string, FittingResult> Results { get; init; }

	internal FittingLogChannelRequest()
	{
	}
	
	public static FittingLogChannelRequest From<TBin, TItem, TParams>(
		List<TBin> bins,
		List<TItem> items,
		TParams parameters,
		IDictionary<string, FittingResult> results
	)
		where TBin: IWithID, IWithReadOnlyDimensions
		where TItem: IWithID, IWithReadOnlyDimensions, IWithQuantity
		where TParams: ILogConvertible
	{
		return new FittingLogChannelRequest()
		{
			Bins = bins.Cast<IWithDimensions>().ToList().AsReadOnly(),
			Items = items.Cast<IWithDimensions>().ToList().AsReadOnly(),
			Parameters = parameters,
			Results = results
		};
	}

}
