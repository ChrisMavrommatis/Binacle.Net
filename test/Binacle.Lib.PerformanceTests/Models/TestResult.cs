using System.Text;
using Binacle.Lib.PerformanceTests.Services;
using ConsoleTables;

namespace Binacle.Lib.PerformanceTests.Models;

internal class TestResult
{
	public required string Title { get; init; }
	public string? Description { get; set; }
	public required string? Filename { get; init; }
	public required IResult Result { get; init; }

	public string ConsolePrint()
	{
		var sb = new StringBuilder();
		sb.AppendLine($"-------- {this.Title} --------");
		sb.AppendLine(string.Empty);

		if (!string.IsNullOrEmpty(this.Description))
		{
			sb.AppendLine(this.Description);
			sb.AppendLine(string.Empty);
		}

		var table = this.GetConsoleTable();
		sb.Append(table.ToMinimalString());
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
		
		var table = this.GetConsoleTable();
		sb.Append( table.ToMarkDownString());
		sb.AppendLine(string.Empty);
		return sb.ToString();
	}
	
	public ConsoleTable GetConsoleTable()
	{
		var dataTable = this.Result.ToDataTable();
		
		var table = ConsoleTable.From(dataTable)
			.Configure(x =>
			{
				x.EnableCount = false;
			});
		return table;
	}
}
