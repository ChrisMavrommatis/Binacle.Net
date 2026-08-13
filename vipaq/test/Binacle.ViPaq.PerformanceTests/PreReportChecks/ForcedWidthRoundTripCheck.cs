using Binacle.ViPaq.Compression;
using Binacle.ViPaq.TestsKernel.Models;
using Binacle.ViPaq.TestsKernel.Providers;

namespace Binacle.ViPaq.PerformanceTests.PreReportChecks;

// Round-trips every real pack forced to 16-bit widths through `ProtocolEncoder` directly — the wrapper only picks
// the narrowest widths, so this is the one path that drives the 16-bit read on the mostly-8-bit real data (a
// too-wide width is conformant). Each codec × both layouts.
internal sealed class ForcedWidthRoundTripCheck : IPreReportCheck
{
	private static readonly ICompressionCodec[] Codecs =
		[new NoOpCodec(), new DeflateCodec(), new GzipCodec()];

	private static readonly Layout[] Layouts = [Layout.RowMajor, Layout.Columnar];

	public void Run()
	{
		AssertRoundTrips(CustomProblemsDataProvider.All);
		AssertRoundTrips(BischoffDataProvider.All);
	}

	private static void AssertRoundTrips(IReadOnlyCollection<Scenario> scenarios)
	{
		foreach (var scenario in scenarios)
		{
			// §4 keeps both item widths Eight for an empty pack, and Encode would reject a forced-wide empty blob.
			if (scenario.ItemCount == 0)
			{
				continue;
			}

			foreach (var layout in Layouts)
			{
				var wideHeader = Header.Create<Dimensions<ushort>, Item<ushort>, ushort>(scenario.Bin, scenario.Items)
					with
					{
						Layout = layout,
						BinDimensionsWidth = Width.Sixteen,
						ItemDimensionsWidth = Width.Sixteen,
						ItemCoordinatesWidth = Width.Sixteen,
					};

				foreach (var codec in Codecs)
				{
					var header = wideHeader with { Compressed = codec is not NoOpCodec };
					var encoder = new ProtocolEncoder(codec);

					var token = encoder.Encode<Dimensions<ushort>, Item<ushort>, ushort>(
						header, 
						scenario.Bin,
						scenario.Items
					);
					
					var (bin, items) = encoder.Decode<Dimensions<ushort>, Item<ushort>, ushort>(
						header, 
						token[Header.ByteCount..]
					);
					RoundTripAssertion.Assert(
						scenario,
						token, 
						header,
						bin,
						items,
						$"{codec.GetType().Name}, {layout}, forced 16-bit"
					);
				}
			}
		}
	}
}
