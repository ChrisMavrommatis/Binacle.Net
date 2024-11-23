using System.Data;
using System.Numerics;
using ConsoleTables;

namespace Binacle.Net.Lib.PerformanceTests.Models;

internal class MeasurementResults<T> : List<MeasurementResult<T>>, IResult
	where T : struct,  INumber<T>, IComparable<T>
{
	public required string Unit { get; init; }

	public DataTable ToDataTable()
	{
		var table = new DataTable();
		table.Columns.Add("Algorithm", typeof(string));
		table.Columns.Add($"Min ({this.Unit})", typeof(T));
		table.Columns.Add($"Mean ({this.Unit})", typeof(double));
		table.Columns.Add($"Median ({this.Unit})", typeof(double));
		table.Columns.Add($"Max ({this.Unit})", typeof(T));

		foreach (var value in this)
		{
			table.Rows.Add(value.Algorithm, value.Min, value.Mean, value.Median, value.Max);
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
