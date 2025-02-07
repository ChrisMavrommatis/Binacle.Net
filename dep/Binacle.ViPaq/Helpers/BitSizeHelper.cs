using System.Numerics;
using Binacle.ViPaq.Abstractions;
using Binacle.ViPaq.Models;

namespace Binacle.ViPaq.Helpers;

internal static class BitSizeHelper
{
	public static BitSize GetCoordinatesBitSize<TObject, T>(
		TObject obj) 
		where T : struct, IBinaryInteger<T>, INumber<T>, IComparable<T>
		where TObject : IWithCoordinates<T> 
	{
		if (obj.X < T.Zero)
		{
			throw new ArgumentOutOfRangeException(
				nameof(obj.X),
				obj.X,
				$"{nameof(obj.X)} must be zero or positive"
				);
		}
		if (obj.Y < T.Zero)
		{
			throw new ArgumentOutOfRangeException(
				nameof(obj.Y),
				obj.Y,
				$"{nameof(obj.Y)} must be zero or positive"
				);
		}
		if (obj.Z < T.Zero)
		{
			throw new ArgumentOutOfRangeException(
				nameof(obj.Z),
				obj.Z,
				$"{nameof(obj.Z)} must be zero or positive"
				);
		}
		
		var uInt8Size = T.CreateChecked(byte.MaxValue);
		
		if(obj.X <= uInt8Size && obj.Y <= uInt8Size && obj.Z <= uInt8Size)
		{
			return BitSize.Eight;
		}
		
		var uInt16Size = T.CreateChecked(ushort.MaxValue);
		if(obj.X <= uInt16Size && obj.Y <= uInt16Size && obj.Z <= uInt16Size)
		{
			return BitSize.Sixteen;
		}
		
		var uInt32Size = T.CreateChecked(uint.MaxValue);
		
		if(obj.X <= uInt32Size && obj.Y <= uInt32Size && obj.Z <= uInt32Size)
		{
			return BitSize.ThirtyTwo;
		}
		
		var uInt64Size = T.CreateChecked(ulong.MaxValue);
		
		if(obj.X <= uInt64Size && obj.Y <= uInt64Size && obj.Z <= uInt64Size)
		{
			return BitSize.SixtyFour;
		}
		
		throw new ArgumentOutOfRangeException(
			$"{nameof(obj)} coordinates",
			$"{obj.X},{obj.Y},{obj.Z}",
			$"The {nameof(obj)} Coordinates are too large"
			);
	}

	public static BitSize GetDimensionsBitSize<TObject, T>(
		TObject obj
	) 
		where T : struct, IBinaryInteger<T>, INumber<T>, IComparable<T>
		where TObject : IWithDimensions<T> 
	{
		if (obj.Length <= T.Zero)
		{
			throw new ArgumentOutOfRangeException(
				nameof(obj.Length),
				obj.Length,
				$"{nameof(obj.Length)} must be greater than zero"
				);
		}
		if (obj.Width <= T.Zero)
		{
			throw new ArgumentOutOfRangeException(
				nameof(obj.Width),
				obj.Width,
				$"{nameof(obj.Width)} must be greater than zero"
				);
		}
		if (obj.Height <= T.Zero)
		{
			throw new ArgumentOutOfRangeException(
				nameof(obj.Height),
				obj.Height,
				$"{nameof(obj.Height)} must be greater than zero"
				);
		}
		
		var uInt8Size = T.CreateChecked(byte.MaxValue);
		
		if(obj.Length <= uInt8Size && obj.Width <= uInt8Size && obj.Height <= uInt8Size)
		{
			return BitSize.Eight;
		}
		
		var uInt16Size = T.CreateChecked(ushort.MaxValue);
		if(obj.Length <= uInt16Size && obj.Width <= uInt16Size && obj.Height <= uInt16Size)
		{
			return BitSize.Sixteen;
		}
		
		var uInt32Size = T.CreateChecked(uint.MaxValue);
		
		if(obj.Length <= uInt32Size && obj.Width <= uInt32Size && obj.Height <= uInt32Size)
		{
			return BitSize.ThirtyTwo;
		}
		
		var uInt64Size = T.CreateChecked(ulong.MaxValue);
		
		if(obj.Length <= uInt64Size && obj.Width <= uInt64Size && obj.Height <= uInt64Size)
		{
			return BitSize.SixtyFour;
		}
		
		throw new ArgumentOutOfRangeException(
			$"{nameof(obj)} dimensions",
			$"{obj.Length}x{obj.Width}x{obj.Height}",
			$"The {nameof(obj)} dimensions are too large"
			);
	}
}
