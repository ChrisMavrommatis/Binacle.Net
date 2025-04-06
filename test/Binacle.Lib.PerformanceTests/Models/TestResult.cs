using System.Text;
using Microsoft.Extensions.Logging;

namespace Binacle.Lib.PerformanceTests.Models;

internal class TestResult
{
	public required string Title { get; set; }
	public string? Description { get; set; }
	public required IResult Result { get; set; }

	public string ConsolePrint()
	{
		var sb = new StringBuilder();
		sb.AppendLine($"-------- Printing Test Result --------");
		sb.AppendLine(string.Empty);

		sb.AppendLine(this.Title);
		if (!string.IsNullOrEmpty(this.Description))
		{
			sb.AppendLine(this.Description);
		}
		sb.AppendLine(string.Empty);

		var result = this.Result.ConsolePrint();
		sb.Append(result);
		sb.AppendLine(string.Empty);
		return sb.ToString();
	}
	
	public string MarkdownPrint()
	{
		var sb = new StringBuilder();
		sb.AppendLine($"## {this.Title}");
		if (!string.IsNullOrEmpty(this.Description))
		{
			sb.AppendLine(this.Description);
		}
		sb.AppendLine(string.Empty);

		var result = this.Result.MarkdownPrint();
		sb.Append(result);
		sb.AppendLine(string.Empty);
		return sb.ToString();
	}
}
