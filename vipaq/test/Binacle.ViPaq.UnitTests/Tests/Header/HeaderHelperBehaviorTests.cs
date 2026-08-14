using Binacle.ViPaq.Helpers;
using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// The header guards: the throws and their "does not throw" boundaries. A type cannot go in a data row, so a
// lookup table turns the type from the row into the real call.
[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class HeaderHelperBehaviorTests
{
	// type from the row -> the call that uses that type.
	private static readonly Dictionary<Type, Action<Header>> Callers = new()
	{
		[typeof(sbyte)]  = header => HeaderHelper.ThrowOnInvalidHeader<sbyte>(header),
		[typeof(byte)]   = header => HeaderHelper.ThrowOnInvalidHeader<byte>(header),
		[typeof(short)]  = header => HeaderHelper.ThrowOnInvalidHeader<short>(header),
		[typeof(ushort)] = header => HeaderHelper.ThrowOnInvalidHeader<ushort>(header),
		[typeof(int)]    = header => HeaderHelper.ThrowOnInvalidHeader<int>(header),
		[typeof(uint)]   = header => HeaderHelper.ThrowOnInvalidHeader<uint>(header),
		[typeof(long)]   = header => HeaderHelper.ThrowOnInvalidHeader<long>(header),
		[typeof(ulong)]  = header => HeaderHelper.ThrowOnInvalidHeader<ulong>(header),
		[typeof(char)]   = header => HeaderHelper.ThrowOnInvalidHeader<char>(header),
		[typeof(nint)]   = header => HeaderHelper.ThrowOnInvalidHeader<nint>(header),
		[typeof(nuint)]  = header => HeaderHelper.ThrowOnInvalidHeader<nuint>(header),
	};

	// Width is internal, so a public [Theory] cannot name it (CS0051): the boxed widths ride the row as object.
	[Theory]
	[ClassData(typeof(HeaderHelperTestCaseProvider))]
	public void ThrowOnInvalidHeader_Behaves_As_Expected(
		Type numericType,
		object binWidthValue,
		object itemDimensionsWidthValue,
		object itemCoordinatesWidthValue,
		Type? expectedError,
		string? expectedFieldName)
	{
		var header = new Header
		{
			Compressed = false,
			Layout = Layout.RowMajor,
			BinDimensionsWidth = (Width)binWidthValue,
			ItemDimensionsWidth = (Width)itemDimensionsWidthValue,
			ItemCoordinatesWidth = (Width)itemCoordinatesWidthValue
		};

		var error = Record.Exception(() => Callers[numericType](header));

		if (expectedError is null)
		{
			error.ShouldBeNull();
			return;
		}

		error.ShouldNotBeNull();
		error.GetType().ShouldBe(expectedError);
		((ArgumentException)error).ParamName.ShouldBe(expectedFieldName);
	}

	[Theory]
	[InlineData((int)ushort.MaxValue, false)]   // 65535 items is allowed (the boundary)
	[InlineData(ushort.MaxValue + 1, true)]     // 65536 items is too many
	public void ThrowIfTooManyItems_Enforces_Item_Count_Limit(int itemCount, bool shouldThrow)
	{
		// The count guard only looks at how many items there are, so fixed values are enough.
		var items = Enumerable.Range(0, itemCount)
			.Select(_ => new Binacle.Geometry.Item<ulong> { Length = 1, Width = 1, Height = 1, X = 0, Y = 0, Z = 0 })
			.ToList();

		var act = () => ValidationHelper.ThrowIfTooManyItems(items);

		if (shouldThrow)
		{
			Should.Throw<ArgumentOutOfRangeException>(act);
		}
		else
		{
			Should.NotThrow(act);
		}
	}
}
