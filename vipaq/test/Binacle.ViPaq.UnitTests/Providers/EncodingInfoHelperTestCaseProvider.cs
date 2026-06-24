using System.Collections;

namespace Binacle.ViPaq.UnitTests.Providers;

// Full matrix for the ThrowOnInvalidEncodingInfo guard.
// Each row is: the numeric type, the three section sizes as "bin-itemDim-itemCoord",
// the error it should throw (null = no error), and the field that error should name.
// Sizes use 8 / 16 / 32 / 64 to mean the four BitSize values.
//
// The rule under test: a type can hold a section only if the type is at least as wide.
// The guard checks bin first, then item dimensions, then item coordinates, and names the
// first section that is too big. The two types of each width (e.g. sbyte and byte) behave
// the same, so both siblings are listed but the sizes are not crossed against every type.
internal class EncodingInfoHelperTestCaseProvider : IEnumerable<object[]>
{
	private static readonly Type argumentException = typeof(ArgumentException);
	private static readonly Type argumentOutOfRangeException = typeof(ArgumentOutOfRangeException);

	private const string binDimensionsBitSize = nameof(EncodingInfo.BinDimensionsBitSize);
	private const string itemDimensionsBitSize = nameof(EncodingInfo.ItemDimensionsBitSize);
	private const string itemCoordinatesBitSize = nameof(EncodingInfo.ItemCoordinatesBitSize);

	// Every supported type accepts the smallest sections -> no error.
	public IEnumerable<object[]> SupportedTypeCases =>
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
	public IEnumerable<object[]> UnsupportedTypeCases =>
	[
		[typeof(char), "8-8-8", argumentException, "T"],
		[typeof(nint), "8-8-8", argumentException, "T"],
		[typeof(nuint), "8-8-8", argumentException, "T"],
	];

	// Type width is exactly the section size -> no error (the boundary; the guard uses "<").
	public IEnumerable<object[]> ExactFitCases =>
	[
		[typeof(short), "16-16-16", null!, null!],
		[typeof(ushort), "16-16-16", null!, null!],
		[typeof(int), "32-32-32", null!, null!],
		[typeof(uint), "32-32-32", null!, null!],
		[typeof(long), "64-64-64", null!, null!],
		[typeof(ulong), "64-64-64", null!, null!],
	];

	// Sections below the type width, in mixed sizes -> no error.
	public IEnumerable<object[]> MixedFitCases =>
	[
		[typeof(int), "16-8-8", null!, null!],
		[typeof(int), "32-16-8", null!, null!],
		[typeof(uint), "8-16-32", null!, null!],
		[typeof(long), "64-32-16", null!, null!],
		[typeof(long), "8-32-64", null!, null!],
		[typeof(ulong), "16-16-16", null!, null!],
	];

	// 8 bit types: any section above 8 is too big -> throws and names that section.
	public IEnumerable<object[]> Section_8Bits_TooBigCases =>
	[
		[typeof(sbyte), "16-8-8", argumentOutOfRangeException, binDimensionsBitSize],
		[typeof(sbyte), "32-8-8", argumentOutOfRangeException, binDimensionsBitSize],
		[typeof(sbyte), "64-8-8", argumentOutOfRangeException, binDimensionsBitSize],
		[typeof(sbyte), "8-16-8", argumentOutOfRangeException, itemDimensionsBitSize],
		[typeof(sbyte), "8-32-8", argumentOutOfRangeException, itemDimensionsBitSize],
		[typeof(sbyte), "8-64-8", argumentOutOfRangeException, itemDimensionsBitSize],
		[typeof(sbyte), "8-8-16", argumentOutOfRangeException, itemCoordinatesBitSize],
		[typeof(sbyte), "8-8-32", argumentOutOfRangeException, itemCoordinatesBitSize],
		[typeof(sbyte), "8-8-64", argumentOutOfRangeException, itemCoordinatesBitSize],

		[typeof(byte), "16-8-8", argumentOutOfRangeException, binDimensionsBitSize],
		[typeof(byte), "32-8-8", argumentOutOfRangeException, binDimensionsBitSize],
		[typeof(byte), "64-8-8", argumentOutOfRangeException, binDimensionsBitSize],
		[typeof(byte), "8-16-8", argumentOutOfRangeException, itemDimensionsBitSize],
		[typeof(byte), "8-32-8", argumentOutOfRangeException, itemDimensionsBitSize],
		[typeof(byte), "8-64-8", argumentOutOfRangeException, itemDimensionsBitSize],
		[typeof(byte), "8-8-16", argumentOutOfRangeException, itemCoordinatesBitSize],
		[typeof(byte), "8-8-32", argumentOutOfRangeException, itemCoordinatesBitSize],
		[typeof(byte), "8-8-64", argumentOutOfRangeException, itemCoordinatesBitSize],
	];

	// 16 bit types: any section above 16 is too big -> throws and names that section.
	public IEnumerable<object[]> Section_16Bits_TooBigCases =>
	[
		[typeof(short), "32-8-8", argumentOutOfRangeException, binDimensionsBitSize],
		[typeof(short), "64-8-8", argumentOutOfRangeException, binDimensionsBitSize],
		[typeof(short), "8-32-8", argumentOutOfRangeException, itemDimensionsBitSize],
		[typeof(short), "8-64-8", argumentOutOfRangeException, itemDimensionsBitSize],
		[typeof(short), "8-8-32", argumentOutOfRangeException, itemCoordinatesBitSize],
		[typeof(short), "8-8-64", argumentOutOfRangeException, itemCoordinatesBitSize],

		[typeof(ushort), "32-8-8", argumentOutOfRangeException, binDimensionsBitSize],
		[typeof(ushort), "64-8-8", argumentOutOfRangeException, binDimensionsBitSize],
		[typeof(ushort), "8-32-8", argumentOutOfRangeException, itemDimensionsBitSize],
		[typeof(ushort), "8-64-8", argumentOutOfRangeException, itemDimensionsBitSize],
		[typeof(ushort), "8-8-32", argumentOutOfRangeException, itemCoordinatesBitSize],
		[typeof(ushort), "8-8-64", argumentOutOfRangeException, itemCoordinatesBitSize],
	];

	// 32 bit types: only a 64 bit section is too big -> throws and names that section.
	public IEnumerable<object[]> Section_32Bits_TooBigCases =>
	[
		[typeof(int), "64-8-8", argumentOutOfRangeException, binDimensionsBitSize],
		[typeof(int), "8-64-8", argumentOutOfRangeException, itemDimensionsBitSize],
		[typeof(int), "8-8-64", argumentOutOfRangeException, itemCoordinatesBitSize],

		[typeof(uint), "64-8-8", argumentOutOfRangeException, binDimensionsBitSize],
		[typeof(uint), "8-64-8", argumentOutOfRangeException, itemDimensionsBitSize],
		[typeof(uint), "8-8-64", argumentOutOfRangeException, itemCoordinatesBitSize],
	];

	// More than one section is too big -> the first one in order (bin, then item dim) is named.
	public IEnumerable<object[]> PrecedenceCases =>
	[
		[typeof(byte), "16-16-8", argumentOutOfRangeException, binDimensionsBitSize],
		[typeof(byte), "8-16-16", argumentOutOfRangeException, itemDimensionsBitSize],
		[typeof(byte), "16-16-16", argumentOutOfRangeException, binDimensionsBitSize],
		[typeof(short), "32-32-8", argumentOutOfRangeException, binDimensionsBitSize],
		[typeof(short), "8-32-32", argumentOutOfRangeException, itemDimensionsBitSize],
		[typeof(int), "64-8-64", argumentOutOfRangeException, binDimensionsBitSize],
	];

	public IEnumerator<object[]> GetEnumerator()
	{
		IEnumerable<object[]> allCases =
		[
			.. SupportedTypeCases,
			.. UnsupportedTypeCases,
			.. ExactFitCases,
			.. MixedFitCases,
			.. Section_8Bits_TooBigCases,
			.. Section_16Bits_TooBigCases,
			.. Section_32Bits_TooBigCases,
			.. PrecedenceCases,
		];
		return allCases.Select(ToTestCase).GetEnumerator();
	}

	// Turn a readable row [type, "8-8-8", error, field] into what the test needs:
	// [type, binSize, itemDimSize, itemCoordSize, error, field].
	private static object[] ToTestCase(object[] row)
	{
		var sizes = ((string)row[1]).Split('-');
		return
		[
			row[0],
			ParseBitSize(sizes[0]),
			ParseBitSize(sizes[1]),
			ParseBitSize(sizes[2]),
			row[2],
			row[3],
		];
	}

	private static BitSize ParseBitSize(string token) => token switch
	{
		"8" => BitSize.Eight,
		"16" => BitSize.Sixteen,
		"32" => BitSize.ThirtyTwo,
		"64" => BitSize.SixtyFour,
		_ => throw new ArgumentException($"Unknown bit size '{token}'"),
	};

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
