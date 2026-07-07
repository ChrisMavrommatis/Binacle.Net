namespace Binacle.TestReporting;

// One report producer. Run it to get a table of results bound to a file. Both the lib and the ViPaq
// performance-test projects implement this; the shared runner and writers do the rest.
public interface ITest
{
	ResultFile File { get; }
	TestResult Run();
}
