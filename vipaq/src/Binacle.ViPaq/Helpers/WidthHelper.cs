using System.Numerics;
using Binacle.Geometry;

namespace Binacle.ViPaq.Helpers;

internal static class WidthHelper
{
	// Bytes on the wire for one integer at this width. Throws on a reserved code (PROTOCOL.md §4).
	public static int ByteCount(Width width)
	{
		return width switch
		{
			Width.Eight => 1,
			Width.Sixteen => 2,
			_ => throw new ArgumentOutOfRangeException(nameof(width), width, $"Width {width} is not supported")
		};
	}

	// The narrowest width that holds all three dimensions. Also the range check: dimensions must be >= 1 and
	// <= MaxValue (PROTOCOL.md §5).
	public static Width GetDimensionsWidth<TObject, T>(TObject obj)
		where T : struct, IBinaryInteger<T>
		where TObject : IWithReadOnlyDimensions<T>
	{
		ThrowIfNotPositive(obj.Length, nameof(obj.Length));
		ThrowIfNotPositive(obj.Width, nameof(obj.Width));
		ThrowIfNotPositive(obj.Height, nameof(obj.Height));

		return WidestOf(
			WidthOf(obj.Length, nameof(obj.Length)),
			WidthOf(obj.Width, nameof(obj.Width)),
			WidthOf(obj.Height, nameof(obj.Height))
			);
	}

	// The narrowest width that holds all three coordinates. Zero is valid: an item flush to the bin origin.
	public static Width GetCoordinatesWidth<TObject, T>(TObject obj)
		where T : struct, IBinaryInteger<T>
		where TObject : IWithReadOnlyCoordinates<T>
	{
		ThrowIfNegative(obj.X, nameof(obj.X));
		ThrowIfNegative(obj.Y, nameof(obj.Y));
		ThrowIfNegative(obj.Z, nameof(obj.Z));

		return WidestOf(
			WidthOf(obj.X, nameof(obj.X)),
			WidthOf(obj.Y, nameof(obj.Y)),
			WidthOf(obj.Z, nameof(obj.Z))
			);
	}

	private static Width WidthOf<T>(T value, string name)
		where T : struct, IBinaryInteger<T>
	{
		if (value <= T.CreateSaturating(Limits.EightBitsMax))
		{
			return Width.Eight;
		}

		if (value <= T.CreateSaturating(Limits.MaxValue))
		{
			return Width.Sixteen;
		}

		throw new ArgumentOutOfRangeException(
			name,
			value,
			$"{name} exceeds the max supported value ({Limits.MaxValue})"
			);
	}

	// Widths are ordered by size, so the wider of two is just the larger code.
	public static Width Widest(Width first, Width second)
	{
		return second > first ? second : first;
	}

	private static Width WidestOf(Width first, Width second, Width third)
	{
		return Widest(Widest(first, second), third);
	}

	private static void ThrowIfNotPositive<T>(T value, string name)
		where T : struct, IBinaryInteger<T>
	{
		if (value <= T.Zero)
		{
			throw new ArgumentOutOfRangeException(name, value, $"{name} must be greater than zero");
		}
	}

	private static void ThrowIfNegative<T>(T value, string name)
		where T : struct, IBinaryInteger<T>
	{
		if (value < T.Zero)
		{
			throw new ArgumentOutOfRangeException(name, value, $"{name} must be zero or positive");
		}
	}
}
