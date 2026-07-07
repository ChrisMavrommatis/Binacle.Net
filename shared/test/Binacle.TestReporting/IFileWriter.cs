namespace Binacle.TestReporting;

// Writes the results for one file. Markdown is the only writer today; the interface leaves room for others.
public interface IFileWriter
{
	Task WriteAsync(ResultFile file, TestResult[] results);
}
