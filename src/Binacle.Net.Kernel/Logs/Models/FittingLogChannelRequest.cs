using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Fitting.Models;

namespace Binacle.Net.Kernel.Logs.Models;

public abstract class FittingLogChannelRequestBase
{
	public required IReadOnlyCollection<IWithReadOnlyDimensions> Bins { get; init; }
	public required IReadOnlyCollection<IWithReadOnlyDimensions> Items { get; init; }
	public ILogConvertible? Parameters { get; set; }
	public required Dictionary<string, FittingResult> Results { get; init; }

	internal FittingLogChannelRequestBase()
	{
	}
}

public class LegacyFittingLogChannelRequest : FittingLogChannelRequestBase
{
	private LegacyFittingLogChannelRequest()
	{
	}
	
	public static LegacyFittingLogChannelRequest From<TBin, TItem, TParams>(
		List<TBin> bins,
		List<TItem> items,
		TParams parameters,
		Dictionary<string, FittingResult> results
	)
		where TBin: IWithID, IWithReadOnlyDimensions
		where TItem: IWithID, IWithReadOnlyDimensions, IWithQuantity
		where TParams: ILogConvertible
	{
		return new LegacyFittingLogChannelRequest()
		{
			Bins = bins.Cast<IWithDimensions>().ToList().AsReadOnly(),
			Items = items.Cast<IWithDimensions>().ToList().AsReadOnly(),
			Parameters = parameters,
			Results = results
		};
	}
}
