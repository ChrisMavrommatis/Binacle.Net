namespace Binacle.OrLibrary.Converter;

// Converts the raw OR-Library text, carried as embedded resources, into the tests-kernel scenario JSON. Takes
// no arguments on purpose: it always runs every converter, so it cannot half-run and leave the data
// inconsistent. Output is deterministic, so a no-change re-run is byte-identical. Regenerating rewrites the
// committed fixtures. Add a converter by implementing IConverter and listing it below.
internal class Program
{
	static void Main(string[] args)
	{
		IConverter[] converters =
		[
			new BischoffSuiteConverter(),
		];

		foreach (var converter in converters)
		{
			converter.Convert();
		}
	}
}
