using Binacle.Lib;

namespace Binacle.ViPaq.PackedDataGenerator;

// Packs the Bischoff suite + custom problems with Binacle.Lib and freezes the *placed* results (bin + items
// with L/W/H and X/Y/Z) as committed data files under vipaq/data/packed (split into bischoff-suite/ and
// custom-problems/). The ViPaq tests-kernel reads those files instead of a hardcoded, API-captured
// RealDataProvider. Takes no arguments on purpose: a run always regenerates every algorithm in the list below,
// so it can't half-run and leave the data mixed. Output is deterministic, so a no-change re-run is
// byte-identical (no git noise).
//
// FFD is the pinned algorithm today. Adding WFD/BFD later is one entry in the list; the algorithm rides on the
// file name as a ".<algo>" suffix (orlib_thpack1.ffd.json), so the sets sit side by side without mixing. Every
// emitted sample must round-trip (encode then decode == input) or the run exits non-zero — data that doesn't
// decode is not valid.
internal class Program
{
	static int Main(string[] args)
	{
		Algorithm[] algorithms =
		[
			Algorithm.FFD,
		];

		var generator = new PackedDataGenerator();
		var allRoundTripped = true;
		foreach (var algorithm in algorithms)
		{
			if (!generator.Generate(algorithm))
			{
				allRoundTripped = false;
			}
		}

		if (!allRoundTripped)
		{
			Console.Error.WriteLine("One or more samples failed to round-trip. The data is not valid; see the log above.");
			return 1;
		}

		return 0;
	}
}
