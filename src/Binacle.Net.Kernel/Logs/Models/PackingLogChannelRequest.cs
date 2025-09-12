using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Packing.Models;

namespace Binacle.Net.Kernel.Logs.Models;

public class PackingLogChannelRequest
{
	public required IReadOnlyCollection<IWithReadOnlyDimensions> Bins { get; init; }
	public required IReadOnlyCollection<IWithReadOnlyDimensions> Items { get; init; }
	public ILogConvertible? Parameters { get; set; }
	public required IDictionary<string, PackingResult> Results { get; init; }
	
	internal PackingLogChannelRequest() 
	{
	}
	
	public static PackingLogChannelRequest From<TBin, TItem, TParams>(
		List<TBin> bins,
		List<TItem> items,
		TParams parameters,
		IDictionary<string, PackingResult> results
	)
		where TBin: IWithID, IWithReadOnlyDimensions
		where TItem: IWithID, IWithReadOnlyDimensions, IWithQuantity
		where TParams: ILogConvertible
	{
		return new PackingLogChannelRequest()
		{
			Bins = bins.Cast<IWithDimensions>().ToList().AsReadOnly(),
			Items =	items.Cast<IWithDimensions>().ToList().AsReadOnly(),
			Parameters = parameters,
			Results = results,
		};
	}
}

