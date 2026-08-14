namespace Binacle.ViPaq.TestsKernel.Models;

// Which form of a token to build, minus the codec. The race always compresses - NoOp prices the raw size - so
// there is no "compressed?" flag, only the layout. `Layout` is a library-internal enum and cannot be a public
// member, so the two forms are handed out as ready-made instances.
public sealed record EncoderInfo
{
	internal Layout Layout { get; init; }

	// How a report names the layout.
	public string LayoutName => this.Layout == Layout.Columnar ? "Columnar" : "Row";

	public static EncoderInfo RowMajor { get; } = new() { Layout = Layout.RowMajor };

	public static EncoderInfo Columnar { get; } = new() { Layout = Layout.Columnar };
}
