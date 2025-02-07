using System.Numerics;
using Binacle.ViPaq.Abstractions;
using Binacle.ViPaq.Models;
using Version = Binacle.ViPaq.Models.Version;

namespace Binacle.ViPaq.Helpers;

internal static class EncodingInfoHelper
{
	private static Dictionary<Type, BitSize> _bitSizes = new()
	{
		{typeof(byte), BitSize.Eight},
		{typeof(sbyte), BitSize.Eight},
		{typeof(short), BitSize.Sixteen},
		{typeof(ushort), BitSize.Sixteen},
		{typeof(int), BitSize.ThirtyTwo},
		{typeof(uint), BitSize.ThirtyTwo},
		{typeof(long), BitSize.SixtyFour},
		{typeof(ulong), BitSize.SixtyFour},
	};
	
	public static EncodingInfo CreateEncodingInfo<TBin, TItem, T>(
		TBin bin, 
		IList<TItem> items
	) 
		where T : struct, IBinaryInteger<T>, INumber<T>, IComparable<T>
		where TBin : IWithDimensions<T>
		where TItem: IWithDimensions<T>, IWithCoordinates<T>
	{
		if(items.Count > ushort.MaxValue)
		{
			throw new ArgumentOutOfRangeException(
				nameof(items),
				items.Count,
				$"{nameof(items)} cannot be more than {ushort.MaxValue}"
				);
		}

		var binDimensionsBitSize = BitSizeHelper.GetDimensionsBitSize<TBin, T>(bin);
		var itemDimensionsBitSize = BitSize.Eight;
		var itemCoordinatesBitSize = BitSize.Eight;
		foreach (var item in items)
		{
			var localItemDimensionsBitSize = BitSizeHelper.GetDimensionsBitSize<TItem, T>(item);
			if(localItemDimensionsBitSize > itemDimensionsBitSize)
			{
				itemDimensionsBitSize = localItemDimensionsBitSize;
			}
			var localItemCoordinatesBitSize = BitSizeHelper.GetCoordinatesBitSize<TItem, T>(item);
			
			if(localItemCoordinatesBitSize > itemCoordinatesBitSize)
			{
				itemCoordinatesBitSize = localItemCoordinatesBitSize;
			}
		}
		
		return new EncodingInfo
		{
			Version = Version.Uncompressed,
			BinDimensionsBitSize = binDimensionsBitSize,
			ItemDimensionsBitSize = itemDimensionsBitSize,
			ItemCoordinatesBitSize = itemCoordinatesBitSize
		};
	}

	public static void ThrowOnInvalidEncodingInfo<T>(EncodingInfo readEncodingInfo) 
		where T : struct, IBinaryInteger<T>, INumber<T>, IComparable<T>
	{
		var typeOfT = typeof(T);
		if (!_bitSizes.TryGetValue(typeOfT, out var bitSize))
		{
			throw new ArgumentException($"Unsupported generic type {typeOfT}", nameof(T));
		}
		
		if(bitSize < readEncodingInfo.BinDimensionsBitSize)
		{
			throw new ArgumentOutOfRangeException(
				nameof(readEncodingInfo.BinDimensionsBitSize),
				bitSize,
				$"Expected the bit size ({bitSize}) of generic parameter to be greater or equal to {nameof(readEncodingInfo.BinDimensionsBitSize)}({readEncodingInfo.BinDimensionsBitSize}).");
		}
		
		if(bitSize < readEncodingInfo.ItemDimensionsBitSize)
		{
			throw new ArgumentOutOfRangeException(
				nameof(readEncodingInfo.ItemDimensionsBitSize),
				bitSize,
				$"Expected the bit size ({bitSize}) of generic parameter to be greater or equal to {nameof(readEncodingInfo.ItemDimensionsBitSize)}({readEncodingInfo.ItemDimensionsBitSize})."
				);
		}
		
		if(bitSize < readEncodingInfo.ItemCoordinatesBitSize)
		{
			throw new ArgumentOutOfRangeException(
				nameof(readEncodingInfo.ItemCoordinatesBitSize),
				bitSize,
				$"Expected the bit size ({bitSize}) of generic parameter to be greater or equal to {nameof(readEncodingInfo.ItemCoordinatesBitSize)}({readEncodingInfo.ItemCoordinatesBitSize})."
				);
		}
	}
}
