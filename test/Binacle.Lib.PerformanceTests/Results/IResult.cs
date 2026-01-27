using System.Data;

namespace Binacle.Lib.PerformanceTests.Services;

internal interface IResult
{
	DataTable ToDataTable();
}

internal class ColumnResult<T> : Dictionary<string, T>
{
}
