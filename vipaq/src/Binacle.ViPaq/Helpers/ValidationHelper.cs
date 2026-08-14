using System.Numerics;
using Binacle.Geometry;

namespace Binacle.ViPaq.Helpers;

// Everything PROTOCOL.md §8 says must be rejected. Encode errors are argument errors, decode errors are
// ViPaqFormatException.
internal static class ValidationHelper
{
	public static void ThrowIfTooManyItems<TItem>(IReadOnlyList<TItem> items)
	{
		if (items.Count > Limits.MaxItemCount)
		{
			throw new ArgumentOutOfRangeException(
				nameof(items),
				items.Count,
				$"{nameof(items)} cannot be more than {Limits.MaxItemCount}"
				);
		}
	}

	// With no items both item widths must be Eight (PROTOCOL.md §4), or four headers would describe the same
	// empty blob and no golden vector could pin one.
	public static void ThrowIfEmptyAndNotEightBits<TItem>(IReadOnlyList<TItem> items, Header header)
	{
		if (items.Count > 0)
		{
			return;
		}

		if (header.ItemDimensionsWidth != Width.Eight)
		{
			throw new ArgumentException("With no items, the item dimensions width must be Eight", nameof(header));
		}

		if (header.ItemCoordinatesWidth != Width.Eight)
		{
			throw new ArgumentException("With no items, the item coordinates width must be Eight", nameof(header));
		}
	}

	// The width helpers do double duty: they reject an out-of-range value, and what they return is the narrowest
	// width that holds the section.
	public static void ThrowIfHeaderCannotHold<TBin, TItem, T>(TBin bin, IReadOnlyList<TItem> items, Header header)
		where T : struct, IBinaryInteger<T>
		where TBin : IWithReadOnlyDimensions<T>
		where TItem : IWithReadOnlyDimensions<T>, IWithReadOnlyCoordinates<T>
	{
		ThrowIfTooNarrow(
			WidthHelper.GetDimensionsWidth<TBin, T>(bin),
			header.BinDimensionsWidth,
			"bin dimensions"
			);

		foreach (var item in items)
		{
			ThrowIfTooNarrow(
				WidthHelper.GetDimensionsWidth<TItem, T>(item),
				header.ItemDimensionsWidth,
				"item dimensions"
				);

			ThrowIfTooNarrow(
				WidthHelper.GetCoordinatesWidth<TItem, T>(item),
				header.ItemCoordinatesWidth,
				"item coordinates"
				);
		}
	}

	public static void ThrowIfNotAWholeBlob(byte[] data)
	{
		if (data is null || data.Length < Header.ByteCount)
		{
			throw new ViPaqFormatException(
				$"A blob is at least {Header.ByteCount} bytes, got {data?.Length ?? 0}"
				);
		}
	}

	// The item count is the body's first two bytes, needed before the expected body length can be worked out.
	public static void ThrowIfBodyHasNoItemCount(byte[] body)
	{
		if (body.Length < sizeof(ushort))
		{
			throw new ViPaqFormatException($"A body is at least {sizeof(ushort)} bytes, got {body.Length}");
		}
	}

	// One check for both truncation and trailing bytes (PROTOCOL.md §7, steps 1 and 8), so no read after it
	// needs a guard. `contents` is the body with its leading uint16 item count already taken off.
	public static void ThrowIfContentsIsNotExactlyAsLongAsDeclared(byte[] contents, Header header, int itemCount)
	{
		var expectedLength = header.GetContentsLength(itemCount);
		if (contents.Length != expectedLength)
		{
			throw new ViPaqFormatException(
				$"Contents declaring {itemCount} items are {expectedLength} bytes at this header, got {contents.Length}"
				);
		}
	}

	private static void ThrowIfTooNarrow(Width required, Width declared, string section)
	{
		if (required > declared)
		{
			throw new ArgumentOutOfRangeException(
				nameof(declared),
				declared,
				$"The declared {section} width ({declared}) cannot hold every value in that section ({required} is needed)"
				);
		}
	}
}
