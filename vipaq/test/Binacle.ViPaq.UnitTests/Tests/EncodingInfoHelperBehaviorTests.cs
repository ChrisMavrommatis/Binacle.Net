using Binacle.ViPaq.UnitTests.Models;
using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// Behaviour of the EncodingInfoHelper guards: the throws and their "does not throw" boundaries.
// The bit-size guard is data driven (see EncodingInfoHelperTestCaseProvider). A type cannot be
// put in a data row, so a small lookup table turns the type from the row into the real call.
[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class EncodingInfoHelperBehaviorTests
{
	// type from the row -> the call that uses that type.
	private static readonly Dictionary<Type, Action<EncodingInfo>> Callers = new()
	{
		[typeof(sbyte)]  = info => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<sbyte>(info),
		[typeof(byte)]   = info => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<byte>(info),
		[typeof(short)]  = info => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<short>(info),
		[typeof(ushort)] = info => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<ushort>(info),
		[typeof(int)]    = info => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<int>(info),
		[typeof(uint)]   = info => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<uint>(info),
		[typeof(long)]   = info => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<long>(info),
		[typeof(ulong)]  = info => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<ulong>(info),
		[typeof(char)]   = info => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<char>(info),
		[typeof(nint)]   = info => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<nint>(info),
		[typeof(nuint)]  = info => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<nuint>(info),
	};

	[Theory]
	[ClassData(typeof(EncodingInfoHelperTestCaseProvider))]
	public void ThrowOnInvalidEncodingInfo_Behaves_As_Expected(
		Type numericType,
		BitSize binSize,
		BitSize itemDimensionsSize,
		BitSize itemCoordinatesSize,
		Type? expectedError,
		string? expectedFieldName)
	{
		var encodingInfo = new EncodingInfo
		{
			Version = Version.Uncompressed,
			BinDimensionsBitSize = binSize,
			ItemDimensionsBitSize = itemDimensionsSize,
			ItemCoordinatesBitSize = itemCoordinatesSize
		};

		var error = Record.Exception(() => Callers[numericType](encodingInfo));

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
	public void CreateEncodingInfo_Enforces_Item_Count_Limit(int itemCount, bool shouldThrow)
	{
		// The count guard only looks at how many items there are, so fixed values are enough.
		var bin = new Bin<ulong> { Length = 1, Width = 1, Height = 1 };
		var items = Enumerable.Range(0, itemCount)
			.Select(_ => new Item<ulong> { Length = 1, Width = 1, Height = 1, X = 0, Y = 0, Z = 0 })
			.ToList();

		var act = () => { EncodingInfoHelper.CreateEncodingInfo<Bin<ulong>, Item<ulong>, ulong>(bin, items); };

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
