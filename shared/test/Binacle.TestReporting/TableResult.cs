using System.Data;

namespace Binacle.TestReporting;

// A plain table of text cells. A report builds its columns and rows directly — enough for any table that
// does not need a bespoke result type.
public class TableResult : IResult
{
	private readonly string[] columns;
	private readonly List<string[]> rows = new();

	public TableResult(params string[] columns)
	{
		this.columns = columns;
	}

	public void AddRow(params string[] cells)
	{
		if (cells.Length != this.columns.Length)
		{
			throw new ArgumentException(
				$"Row has {cells.Length} cells but the table has {this.columns.Length} columns.",
				nameof(cells));
		}

		this.rows.Add(cells);
	}

	public DataTable ToDataTable()
	{
		var table = new DataTable();
		foreach (var column in this.columns)
		{
			table.Columns.Add(column, typeof(string));
		}

		foreach (var row in this.rows)
		{
			table.Rows.Add(row.Cast<object>().ToArray());
		}

		return table;
	}
}
