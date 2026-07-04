using System.Numerics;
using Binacle.ViPaq.Abstractions;

namespace Binacle.ViPaq.Helpers;

internal static class BitSizeHelper
{
	public static BitSize GetCoordinatesBitSize<TObject, T>(
		TObject obj) 
		where T : struct, IBinaryInteger<T>
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
		
		var eightBitsMax = T.CreateSaturating(ViPaqLimits.EightBitsMax);
		
		if(obj.X <= eightBitsMax && obj.Y <= eightBitsMax && obj.Z <= eightBitsMax)
		{
			return BitSize.Eight;
		}
		
		var sixteenBitsMax = T.CreateSaturating(ViPaqLimits.SixteenBitsMax);
		if(obj.X <= sixteenBitsMax && obj.Y <= sixteenBitsMax && obj.Z <= sixteenBitsMax)
		{
			return BitSize.Sixteen;
		}
		
		var thirtyTwoBitsMax = T.CreateSaturating(ViPaqLimits.ThirtyTwoBitsMax);
		
		if(obj.X <= thirtyTwoBitsMax && obj.Y <= thirtyTwoBitsMax && obj.Z <= thirtyTwoBitsMax)
		{
			return BitSize.ThirtyTwo;
		}
		
		var maxInteger = T.CreateSaturating(ViPaqLimits.MaxInteger);

		if(obj.X <= maxInteger && obj.Y <= maxInteger && obj.Z <= maxInteger)
		{
			return BitSize.SixtyFour;
		}

		// At least one field is above MaxInteger (2^53 - 1) — outside ViPaq's range (PROTOCOL.md §5).
		// Name the offender, like the checks above.
		if (obj.X > maxInteger)
		{
			throw new ArgumentOutOfRangeException(
				nameof(obj.X),
				obj.X,
				$"{nameof(obj.X)} exceeds the max supported value ({ViPaqLimits.MaxInteger})"
				);
		}
		if (obj.Y > maxInteger)
		{
			throw new ArgumentOutOfRangeException(
				nameof(obj.Y),
				obj.Y,
				$"{nameof(obj.Y)} exceeds the max supported value ({ViPaqLimits.MaxInteger})"
				);
		}
		throw new ArgumentOutOfRangeException(
			nameof(obj.Z),
			obj.Z,
			$"{nameof(obj.Z)} exceeds the max supported value ({ViPaqLimits.MaxInteger})"
			);
	}

	public static BitSize GetDimensionsBitSize<TObject, T>(
		TObject obj
	) 
		where T : struct, IBinaryInteger<T>
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
		
		var eightBitsMax = T.CreateSaturating(ViPaqLimits.EightBitsMax);
		
		if(obj.Length <= eightBitsMax && obj.Width <= eightBitsMax && obj.Height <= eightBitsMax)
		{
			return BitSize.Eight;
		}
		
		var sixteenBitsMax = T.CreateSaturating(ViPaqLimits.SixteenBitsMax);
		if(obj.Length <= sixteenBitsMax && obj.Width <= sixteenBitsMax && obj.Height <= sixteenBitsMax)
		{
			return BitSize.Sixteen;
		}
		
		var thirtyTwoBitsMax = T.CreateSaturating(ViPaqLimits.ThirtyTwoBitsMax);
		
		if(obj.Length <= thirtyTwoBitsMax && obj.Width <= thirtyTwoBitsMax && obj.Height <= thirtyTwoBitsMax)
		{
			return BitSize.ThirtyTwo;
		}
		
		var maxInteger = T.CreateSaturating(ViPaqLimits.MaxInteger);

		if(obj.Length <= maxInteger && obj.Width <= maxInteger && obj.Height <= maxInteger)
		{
			return BitSize.SixtyFour;
		}

		// At least one field is above MaxInteger (2^53 - 1) — outside ViPaq's range (PROTOCOL.md §5).
		// Name the offender, like the checks above.
		if (obj.Length > maxInteger)
		{
			throw new ArgumentOutOfRangeException(
				nameof(obj.Length),
				obj.Length,
				$"{nameof(obj.Length)} exceeds the max supported value ({ViPaqLimits.MaxInteger})"
				);
		}
		if (obj.Width > maxInteger)
		{
			throw new ArgumentOutOfRangeException(
				nameof(obj.Width),
				obj.Width,
				$"{nameof(obj.Width)} exceeds the max supported value ({ViPaqLimits.MaxInteger})"
				);
		}
		throw new ArgumentOutOfRangeException(
			nameof(obj.Height),
			obj.Height,
			$"{nameof(obj.Height)} exceeds the max supported value ({ViPaqLimits.MaxInteger})"
			);
	}
}
