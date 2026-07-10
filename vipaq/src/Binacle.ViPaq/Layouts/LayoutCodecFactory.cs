using System.Numerics;

namespace Binacle.ViPaq.Layouts;

// Builds the codec for a Layout bit. Both codecs are stateless, so each is made once and shared. One instance
// answers both calls — the split is only about which half of it the caller is handed, and so which constraints
// its items must meet.
internal static class LayoutCodecFactory<T>
	where T : struct, IBinaryInteger<T>
{
	private static readonly RowMajorCodec<T> rowMajor = new();
	private static readonly ColumnarCodec<T> columnar = new();

	public static ILayoutEncoder<T> CreateEncoder(Layout layout)
	{
		return layout switch
		{
			Layout.RowMajor => rowMajor,
			Layout.Columnar => columnar,
			_ => throw new ArgumentOutOfRangeException(nameof(layout), layout, $"Layout {layout} is not supported")
		};
	}

	public static ILayoutDecoder<T> CreateDecoder(Layout layout)
	{
		return layout switch
		{
			Layout.RowMajor => rowMajor,
			Layout.Columnar => columnar,
			_ => throw new ArgumentOutOfRangeException(nameof(layout), layout, $"Layout {layout} is not supported")
		};
	}
}
