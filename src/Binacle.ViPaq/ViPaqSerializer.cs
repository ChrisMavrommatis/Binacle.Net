using Binacle.ViPaq.Abstractions;

namespace Binacle.ViPaq;

public static partial class ViPaqSerializer
{
	public static (TBin, IList<TItem>) DeserializeInt32<TBin, TItem>(byte[] data)
		where TBin : IWithDimensions<int>, new()
		where TItem : IWithDimensions<int>, IWithCoordinates<int>, new()
	{
		return Deserialize<TBin, TItem, int>(data);
	}

	public static (TBin, IList<TItem>) DeserializeUInt16<TBin, TItem>(byte[] data)
		where TBin : IWithDimensions<ushort>, new()
		where TItem : IWithDimensions<ushort>, IWithCoordinates<ushort>, new()
	{
		return Deserialize<TBin, TItem, ushort>(data);
	}
	
	
	public static byte[] SerializeInt32<TBin, TItem>(TBin bin, IList<TItem> items)
		where TBin : IWithDimensions<int>
		where TItem : IWithDimensions<int>, IWithCoordinates<int>
	{
		return Serialize<TBin, TItem, int>(bin, items);
	}

	public static byte[] SerializeUInt16<TBin, TItem>(TBin bin, IList<TItem> items)
		where TBin : IWithDimensions<ushort>
		where TItem : IWithDimensions<ushort>, IWithCoordinates<ushort>
	{
		return Serialize<TBin, TItem, ushort>(bin, items);
	}
}
