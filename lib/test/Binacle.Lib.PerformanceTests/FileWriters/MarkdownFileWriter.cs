using Binacle.Lib.PerformanceTests.Models;

namespace Binacle.Lib.PerformanceTests.Services;

internal class MarkdownFileWriter : IFileWriter
{
	public async Task WriteAsync(Models.ResultFile file, TestResult[] results)
	{
		var projectRoot = Path.Combine(
			AppDomain.CurrentDomain.BaseDirectory, 
			"..", "..", ".."
		);
		
		var filepath = Path.Combine(
			projectRoot, 
			"PerformanceTests.Artifacts", 
			$"{file.Filename}.md"
		);

		var directoryName = Path.GetDirectoryName(filepath)!;
		//ensure directory exists
		Directory.CreateDirectory(directoryName);

		if (File.Exists(filepath))
		{
			File.Delete(filepath);
		}

		using var writer = new StreamWriter(filepath);

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
