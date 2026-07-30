using System.Data;

namespace Binacle.TestReporting;

// A result the writer can render: it turns into a DataTable, which ConsoleTables prints as markdown or a
// console table. Concrete shapes (a plain string table, or a domain-specific one) implement this.
public interface IResult
{
	DataTable ToDataTable();
}
