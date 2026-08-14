using System.Numerics;

namespace Binacle.ViPaq.Layouts;

// Builds the codec for a Layout bit. Both codecs are stateless, so each is made once and shared; one instance
// answers both calls, and the split only decides which half the caller is handed.
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
