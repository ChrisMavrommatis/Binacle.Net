using System.Numerics;

namespace Binacle.ViPaq.Helpers;

// What a header cannot answer on its own: whether the caller's generic argument is wide enough to hold what
// the header declares. That is a question about `T`, not about the header, so it does not live on `Header`.
internal static class HeaderHelper
{
	// The widest width each generic argument can hold. A blob with 16-bit sections cannot be read into a byte.
	private static readonly Dictionary<Type, Width> widthsByType = new()
	{
		{ typeof(byte), Width.Eight },
		{ typeof(sbyte), Width.Eight },
		{ typeof(short), Width.Sixteen },
		{ typeof(ushort), Width.Sixteen },
		{ typeof(int), Width.Sixteen },
		{ typeof(uint), Width.Sixteen },
		{ typeof(long), Width.Sixteen },
		{ typeof(ulong), Width.Sixteen },
	};

	public static void ThrowOnInvalidHeader<T>(Header header)
		where T : struct, IBinaryInteger<T>
	{
		var typeOfT = typeof(T);
		if (!widthsByType.TryGetValue(typeOfT, out var widest))
		{
			throw new ArgumentException($"Unsupported generic type {typeOfT}", nameof(T));
		}

		ThrowIfTooNarrowForT(widest, header.BinDimensionsWidth, nameof(header.BinDimensionsWidth));
		ThrowIfTooNarrowForT(widest, header.ItemDimensionsWidth, nameof(header.ItemDimensionsWidth));
		ThrowIfTooNarrowForT(widest, header.ItemCoordinatesWidth, nameof(header.ItemCoordinatesWidth));
	}

	private static void ThrowIfTooNarrowForT(Width widest, Width declared, string section)
	{
		if (widest < declared)
		{
			throw new ArgumentOutOfRangeException(
				section,
				declared,
				$"The generic parameter holds at most {widest}, but {section} declares {declared}."
				);
		}
	}
}
