using Binacle.Net.Lib.PerformanceTests.Models;

namespace Binacle.Net.Lib.PerformanceTests.Services;

internal class MarkdownFileWriter
{
	public async Task WriteAsync(TestResultList testResultList)
	{
		var filepath = $"./PerformanceTestsArtifacts/{testResultList.Filename}.md";

		var directoryName = Path.GetDirectoryName(filepath)!;
		//ensure directory exists
		Directory.CreateDirectory(directoryName);
		
		if(File.Exists(filepath))
		{
			File.Delete(filepath);
		}
		
		using var writer = new StreamWriter(filepath);

		await writer.WriteLineAsync($"# {testResultList.Title}");
		await writer.WriteLineAsync(string.Empty);
		
		if (!string.IsNullOrWhiteSpace(testResultList.Description))
		{
			await writer.WriteLineAsync(testResultList.Description);
			await writer.WriteLineAsync(string.Empty);
		}
		
		foreach (var testResult in testResultList)
		{
			await writer.WriteLineAsync(string.Empty);
			await writer.WriteAsync(testResult.MarkdownPrint());
			await writer.WriteLineAsync(string.Empty);
		}
	}
}
