using Binacle.Lib.PerformanceTests.Models;

namespace Binacle.Lib.PerformanceTests.Services;

internal class MarkdownFileWriter : IFileWriter
{
	public async Task WriteAsync(TestResult result)
	{
		var projectRoot = Path.Combine(
			AppDomain.CurrentDomain.BaseDirectory, 
			"..", "..", ".."
		);
		
		var filepath = Path.Combine(
			projectRoot, 
			"PerformanceTests.Artifacts", 
			$"{result.Filename}.md"
		);

		var directoryName = Path.GetDirectoryName(filepath)!;
		//ensure directory exists
		Directory.CreateDirectory(directoryName);

		if (File.Exists(filepath))
		{
			File.Delete(filepath);
		}

		using var writer = new StreamWriter(filepath);

		await writer.WriteLineAsync($"# {result.Title}");
		await writer.WriteLineAsync(string.Empty);

		if (!string.IsNullOrWhiteSpace(result.Description))
		{
			await writer.WriteLineAsync(result.Description);
			await writer.WriteLineAsync(string.Empty);
		}

		await writer.WriteLineAsync(string.Empty);
		await writer.WriteAsync(result.MarkdownPrint());
		await writer.WriteLineAsync(string.Empty);
	}
}
