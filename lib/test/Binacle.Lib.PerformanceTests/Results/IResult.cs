using System.Data;

namespace Binacle.Lib.PerformanceTests.Results;

internal interface IResult
{
	DataTable ToDataTable();
}
