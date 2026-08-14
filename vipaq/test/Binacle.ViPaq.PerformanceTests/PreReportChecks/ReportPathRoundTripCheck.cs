using Binacle.ViPaq.Compression;
using Binacle.ViPaq.TestsKernel.Models;
using Binacle.ViPaq.TestsKernel.Providers;
using Binacle.ViPaq.TestsKernel.ViPaq;

namespace Binacle.ViPaq.PerformanceTests.PreReportChecks;

// Round-trips every real pack through the same encoder the size reports use, so a codec, layout or wrapper bug
// fails here instead of skewing a report.
internal sealed class ReportPathRoundTripCheck : IPreReportCheck
{
	private static readonly ICompressionCodec[] Codecs =
		[new NoOpCodec(), new DeflateCodec(), new GzipCodec()];

	// `EncoderInfo.Layout` is internal to the kernel, so the library `Layout` has to be carried alongside it.
	private static readonly (Layout Layout, EncoderInfo Info)[] Modes = [
		(Layout.RowMajor, EncoderInfo.RowMajor),
		(Layout.Columnar, EncoderInfo.Columnar)
	];

	public void Run()
	{
		AssertRoundTrips(CustomProblemsDataProvider.All);
		AssertRoundTrips(BischoffDataProvider.All);
	}

	private static void AssertRoundTrips(IReadOnlyCollection<Scenario> scenarios)
	{
		foreach (var scenario in scenarios)
		{
			foreach (var (layout, info) in Modes)
			{
				// Mirrors ViPaqHeader.Create: narrowest widths, Compressed always set.
				var expectedHeader = Header.Create<Dimensions<ushort>, Item<ushort>, ushort>(scenario.Bin, scenario.Items)
					with { Compressed = true, Layout = layout };

				foreach (var codec in Codecs)
				{
					var encoder = new ViPaqEncoder(codec);
					var token = encoder.Encode(scenario, info);
					var header = ViPaqHeader.Read(token);
					var (bin, items) = encoder.Decode(token, header);
					RoundTripAssertion.Assert(
						scenario,
						token,
						expectedHeader,
						bin,
						items,
						$"{codec.GetType().Name}, {layout}, natural"
					);
				}
			}
		}
	}
}
