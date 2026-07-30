namespace Binacle.TestReporting;

// Writes each report to <outputDirectory>/<Filename>.md. The directory is passed in, so each project points
// it wherever its results live (e.g. a repo-level results folder). One file per report, overwritten each
// run — the file is the recorded baseline the next run is diffed against.
public class MarkdownFileWriter : IFileWriter
{
	private readonly string outputDirectory;

	public MarkdownFileWriter(string outputDirectory)
	{
		this.outputDirectory = outputDirectory;
	}

	public async Task WriteAsync(ResultFile file, TestResult[] results)
	{
		Directory.CreateDirectory(this.outputDirectory);

		var filepath = Path.Combine(this.outputDirectory, $"{file.Filename}.md");
		if (File.Exists(filepath))
		{
			File.Delete(filepath);
		}

		await using var writer = new StreamWriter(filepath);

		await writer.WriteLineAsync($"# {file.Title}");
		await writer.WriteLineAsync(string.Empty);

		if (!string.IsNullOrWhiteSpace(file.Description))
		{
			await writer.WriteLineAsync(file.Description);
			await writer.WriteLineAsync(string.Empty);
		}

		foreach (var result in results)
		{
			await writer.WriteLineAsync(string.Empty);
			await writer.WriteAsync(result.MarkdownPrint());
			await writer.WriteLineAsync(string.Empty);
		}
	}
}
