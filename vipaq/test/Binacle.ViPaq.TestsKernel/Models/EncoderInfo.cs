namespace Binacle.ViPaq.TestsKernel.Models;

// Which form of a token to build, minus the codec. The codec race always compresses (NoOp is how it prices the
// raw size, by passing the body through), so there is no "compressed?" flag here — only the layout, which is
// ViPaq's alone. `Layout` is a library-internal enum, so it cannot be a public member; the two forms are handed
// out as ready-made instances instead, which is also all a report ever needs.
public sealed record EncoderInfo
{
	internal Layout Layout { get; init; }

	// How a report names the layout.
	public string LayoutName => this.Layout == Layout.Columnar ? "Columnar" : "Row";

	public static EncoderInfo RowMajor { get; } = new() { Layout = Layout.RowMajor };

	public static EncoderInfo Columnar { get; } = new() { Layout = Layout.Columnar };
}
