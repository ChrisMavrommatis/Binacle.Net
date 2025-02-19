using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Fitting.Models;
using Binacle.Net.Lib.Packing.Models;

namespace Binacle.Net.Api.Kernel.Models;

public abstract class PackingLogChannelRequestBase
{
	public required Dictionary<string, PackingResult> Results { get; init; }
	public required IReadOnlyCollection<IWithReadOnlyDimensions> Bins { get; init; }
	public required IReadOnlyCollection<IWithReadOnlyDimensions> Items { get; init; }

	internal PackingLogChannelRequestBase() 
	{
	}
}

public class LegacyPackingLogChannelRequest : PackingLogChannelRequestBase
{
	public static LegacyPackingLogChannelRequest From<TBin, TItem>(
		List<TBin> bins,
		List<TItem> items,
		object parameters,
		Dictionary<string, PackingResult> results
	)
		where TBin: IWithID, IWithReadOnlyDimensions
		where TItem: IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		return new LegacyPackingLogChannelRequest()
		{
			Results = results,
			Bins = bins.Cast<IWithDimensions>().ToList().AsReadOnly(),
			Items =	items.Cast<IWithDimensions>().ToList().AsReadOnly(),
		};
	}
}

public class PackingLogChannelRequest : PackingLogChannelRequestBase
{
	public static PackingLogChannelRequest From<TBin, TItem>(
		List<TBin> bins,
		List<TItem> items,
		object parameters,
		Dictionary<string, PackingResult> results
	)
		where TBin: IWithID, IWithReadOnlyDimensions
		where TItem: IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		return new PackingLogChannelRequest()
		{
			Results = results,
			Bins = bins.Cast<IWithDimensions>().ToList().AsReadOnly(),
			Items =	items.Cast<IWithDimensions>().ToList().AsReadOnly(),
		};
	}
}

public abstract class FittingLogChannelRequestBase
{
	public required Dictionary<string, FittingResult> Results { get; init; }
	public required IReadOnlyCollection<IWithReadOnlyDimensions> Bins { get; init; }
	public required IReadOnlyCollection<IWithReadOnlyDimensions> Items { get; init; }

	internal FittingLogChannelRequestBase()
	{
	}
}

public class LegacyFittingLogChannelRequest : FittingLogChannelRequestBase
{
	private LegacyFittingLogChannelRequest()
	{
	}
	
	public static LegacyFittingLogChannelRequest From<TBin, TItem>(
		List<TBin> bins,
		List<TItem> items,
		object parameters,
		Dictionary<string, FittingResult> results
		)
		where TBin: IWithID, IWithReadOnlyDimensions
		where TItem: IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		return new LegacyFittingLogChannelRequest()
		{
			Results = results,
			Bins = bins.Cast<IWithDimensions>().ToList().AsReadOnly(),
			Items =	items.Cast<IWithDimensions>().ToList().AsReadOnly()
		};
	}
}
