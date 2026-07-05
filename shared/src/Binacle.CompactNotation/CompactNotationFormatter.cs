using System.Numerics;
using System.Text;
using Binacle.Geometry;

namespace Binacle.CompactNotation;

// Formats geometry into the compact text notation — the inverse of CompactNotationParser. Format appends a
// block per interface the value carries: dimensions -> "LxWxH", coordinates -> " (X,Y,Z)",
// quantity -> " [Q]". It reads through the read-only interfaces, so both mutable and immutable objects work.
public static class CompactNotationFormatter
{
	// [CompactFormatterDecision] Prefer the typed composites below (FormatItem / FormatDimensionsAndQuantity) or
	// the single-block primitives — they are compile-time guaranteed. This runtime-polymorphic Format<T> is kept
	// ONLY for the one genuinely type-erased boundary: the Kernel log request echo, where
	// AlgorithmOperationLogChannelRequest types Bins/Items as IReadOnlyCollection<IWithReadOnlyDimensions>, so an
	// item's quantity isn't compile-visible. OPEN (revisit): eliminate this runtime path by (a) a combined
	// dims+quantity read-only interface on the log item DTOs, (b) logging the request echo dims-only (drop [Q]),
	// or (c) leaving it. See .agents/plans/shared-geometry-extraction.md.
	//
	// Formats the dimensions block, then appends the optional coordinates and quantity blocks the value also
	// carries: dimensions -> "LxWxH", + coordinates -> " (X,Y,Z)", + quantity -> " [Q]". Anchoring on
	// IWithReadOnlyDimensions<T> keeps the entry type-safe (no plain object, no runtime "no block" throw).
	public static string Format<T>(IWithReadOnlyDimensions<T> value)
		where T : struct, IBinaryInteger<T>
	{
		var builder = new StringBuilder();
		builder.Append(FormatDimensions(value));

		if (value is IWithReadOnlyCoordinates<T> coordinates)
			AppendBlock(builder, FormatCoordinates(coordinates));

		if (value is IWithReadOnlyQuantity<T> quantity)
			AppendBlock(builder, FormatQuantity(quantity));

		return builder.ToString();
	}

	public static string FormatDimensions<T>(IWithReadOnlyDimensions<T> dimensions)
		where T : struct, IBinaryInteger<T>
		=> $"{dimensions.Length}x{dimensions.Width}x{dimensions.Height}";

	public static string FormatCoordinates<T>(IWithReadOnlyCoordinates<T> coordinates)
		where T : struct, IBinaryInteger<T>
		=> $"({coordinates.X},{coordinates.Y},{coordinates.Z})";

	public static string FormatQuantity<T>(IWithReadOnlyQuantity<T> quantity)
		where T : struct, IBinaryInteger<T>
		=> $"[{quantity.Quantity}]";

	// Concise, compile-guaranteed composites (mirror the parser's ParseItem / ParseDimensionsAndQuantity).
	// TValue is inferred from the argument, so callers write FormatItem(x) with no type args; the constraints
	// guarantee at compile time that x actually carries both blocks (a value missing one won't compile).

	// "LxWxH (X,Y,Z)" — dimensions plus a placement.
	public static string FormatItem<TValue>(TValue value)
		where TValue : IWithReadOnlyDimensions<int>, IWithReadOnlyCoordinates<int>
		=> $"{FormatDimensions(value)} {FormatCoordinates(value)}";

	// "LxWxH [Q]" — dimensions plus a count.
	public static string FormatDimensionsAndQuantity<TValue>(TValue value)
		where TValue : IWithReadOnlyDimensions<int>, IWithReadOnlyQuantity<int>
		=> $"{FormatDimensions(value)} {FormatQuantity(value)}";

	private static void AppendBlock(StringBuilder builder, string block)
	{
		if (builder.Length > 0)
			builder.Append(' ');

		builder.Append(block);
	}
}
