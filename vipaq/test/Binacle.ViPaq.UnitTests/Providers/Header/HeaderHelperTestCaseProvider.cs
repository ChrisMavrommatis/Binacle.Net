using System.Collections;

namespace Binacle.ViPaq.UnitTests.Providers;

// Full matrix for the HeaderHelper.ThrowOnInvalidHeader<T> guard. Each row is: the numeric type, the three
// section widths as "bin-itemDim-itemCoord", the error it should throw (null = no error), and the field that
// error should name.
//
// The rule under test: a type holds a section only if it is at least as wide. byte/sbyte hold Eight; every
// other supported type holds Sixteen. The guard checks bin, then item dimensions, then item coordinates, and
// names the first section that is too wide.
internal class HeaderHelperTestCaseProvider : IEnumerable<object[]>
{
	private static readonly Type argumentException = typeof(ArgumentException);
	private static readonly Type argumentOutOfRangeException = typeof(ArgumentOutOfRangeException);

	private const string binDimensionsWidth = nameof(Header.BinDimensionsWidth);
	private const string itemDimensionsWidth = nameof(Header.ItemDimensionsWidth);
	private const string itemCoordinatesWidth = nameof(Header.ItemCoordinatesWidth);

	// Every supported type accepts the smallest sections -> no error.
	public static IEnumerable<object[]> SupportedTypeCases =>
	[
		[typeof(sbyte), "8-8-8", null!, null!],
		[typeof(byte), "8-8-8", null!, null!],
		[typeof(short), "8-8-8", null!, null!],
		[typeof(ushort), "8-8-8", null!, null!],
		[typeof(int), "8-8-8", null!, null!],
		[typeof(uint), "8-8-8", null!, null!],
		[typeof(long), "8-8-8", null!, null!],
		[typeof(ulong), "8-8-8", null!, null!],
	];

	// A type the format does not support -> ArgumentException naming the generic parameter.
	public static IEnumerable<object[]> UnsupportedTypeCases =>
	[
		[typeof(char), "8-8-8", argumentException, "T"],
		[typeof(nint), "8-8-8", argumentException, "T"],
		[typeof(nuint), "8-8-8", argumentException, "T"],
	];

	// Type width exactly the section width -> no error. The boundary: the guard uses "<".
	public static IEnumerable<object[]> ExactFitCases =>
	[
		[typeof(short), "16-16-16", null!, null!],
		[typeof(ushort), "16-16-16", null!, null!],
		[typeof(int), "16-16-16", null!, null!],
		[typeof(uint), "16-16-16", null!, null!],
		[typeof(long), "16-16-16", null!, null!],
		[typeof(ulong), "16-16-16", null!, null!],
	];

	// Sections below the type width, in mixed widths -> no error.
	public static IEnumerable<object[]> MixedFitCases =>
	[
		[typeof(int), "16-8-8", null!, null!],
		[typeof(int), "8-16-8", null!, null!],
		[typeof(uint), "8-8-16", null!, null!],
		[typeof(long), "16-8-16", null!, null!],
		[typeof(ulong), "8-16-16", null!, null!],
	];

	// 8 bit types: any section above 8 is too wide -> throws and names that section.
	public static IEnumerable<object[]> Section_8Bits_TooWideCases =>
	[
		[typeof(sbyte), "16-8-8", argumentOutOfRangeException, binDimensionsWidth],
		[typeof(sbyte), "8-16-8", argumentOutOfRangeException, itemDimensionsWidth],
		[typeof(sbyte), "8-8-16", argumentOutOfRangeException, itemCoordinatesWidth],

		[typeof(byte), "16-8-8", argumentOutOfRangeException, binDimensionsWidth],
		[typeof(byte), "8-16-8", argumentOutOfRangeException, itemDimensionsWidth],
		[typeof(byte), "8-8-16", argumentOutOfRangeException, itemCoordinatesWidth],
	];

	// More than one section is too wide -> the first one in order (bin, then item dim) is named.
	public static IEnumerable<object[]> PrecedenceCases =>
	[
		[typeof(byte), "16-16-8", argumentOutOfRangeException, binDimensionsWidth],
		[typeof(byte), "8-16-16", argumentOutOfRangeException, itemDimensionsWidth],
		[typeof(byte), "16-16-16", argumentOutOfRangeException, binDimensionsWidth],
	];

	public IEnumerator<object[]> GetEnumerator()
	{
		IEnumerable<object[]> allCases =
		[
			.. SupportedTypeCases,
			.. UnsupportedTypeCases,
			.. ExactFitCases,
			.. MixedFitCases,
			.. Section_8Bits_TooWideCases,
			.. PrecedenceCases,
		];
		return allCases.Select(ToTestCase).GetEnumerator();
	}

	// [type, "8-8-8", error, field] -> [type, binWidth, itemDimWidth, itemCoordWidth, error, field].
	private static object[] ToTestCase(object[] row)
	{
		var widths = ((string)row[1]).Split('-');
		return
		[
			row[0],
			ParseWidth(widths[0]),
			ParseWidth(widths[1]),
			ParseWidth(widths[2]),
			row[2],
			row[3],
		];
	}

	private static Width ParseWidth(string token) => token switch
	{
		"8" => Width.Eight,
		"16" => Width.Sixteen,
		_ => throw new ArgumentException($"Unknown width '{token}'"),
	};

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
