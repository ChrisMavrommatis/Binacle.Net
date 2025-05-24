using System.Data;
using System.Numerics;
using ConsoleTables;

namespace Binacle.Lib.PerformanceTests.Models;

internal class DiscrepancyResults<T> : IResult
	where T : struct,  INumber<T>, IComparable<T>
{
	public required string Unit { get; init; }
	public required string Family { get; init; }
	public required string[] Algorithms { get; init; }
	public required Dictionary<string, Dictionary<string, T>> ScenarioDiscrepancies { get; set; }

	public DataTable ToDataTable()
	{
		var table = new DataTable();
		table.Columns.Add("Scenario", typeof(string));
		foreach (var algorithm in this.Algorithms)
		{
			table.Columns.Add($"{algorithm} ({this.Unit})", typeof(T));
		}

		foreach (var (scenario, discrepancies) in this.ScenarioDiscrepancies)
		{
			var row = new object[this.Algorithms.Length + 1];
			row[0] = scenario;
			for (int i = 0; i < this.Algorithms.Length; i++)
			{
				if (discrepancies.TryGetValue(this.Algorithms[i], out var value))
				{
					row[i + 1] = value;
				}
				else
				{
					row[i + 1] = DBNull.Value; // or some default value
				}
			}
			table.Rows.Add(row);
		}

		return table;
	}

	public string ConsolePrint()
	{
		var dataTable = this.ToDataTable();
		var table = ConsoleTable.From(dataTable)
			.Configure(x =>
			{
				x.EnableCount = false;
			});

		return table.ToMinimalString();
	}

	public string MarkdownPrint()
	{
		var dataTable = this.ToDataTable();
		var table = ConsoleTable.From(dataTable)
			.Configure(x =>
			{
				x.EnableCount = false;
			});

		return table.ToMarkDownString();
	}
}
