using System.Data;
using System.Numerics;

namespace Binacle.Lib.PerformanceTests.Results;

internal class ScenarioCollectionResult<T> : IResult
	where T :  struct, INumber<T>, IComparable<T>
{
	private readonly string resultColumnName;
	private readonly IDictionary<string, AlgorithmResult<T>> results;

	public ScenarioCollectionResult(string resultColumnName)
	{
		this.resultColumnName = resultColumnName;
		this.results = new Dictionary<string, AlgorithmResult<T>>();
	}
	
	public void Add(string resultKey, AlgorithmResult<T> algorithmResult)
	{
		this.results[resultKey] = algorithmResult;
	}
	
	public DataTable ToDataTable()
	{
		var table = new DataTable();
		table.Columns.Add(resultColumnName, typeof(string));
		foreach (var key in this.results.Values.SelectMany(x => x.Keys).Distinct())
		{
			table.Columns.Add(key, typeof(T));
		}
		foreach (var (resultKey, results) in this.results)
		{
			var noOfValues = results.Count + 1;
			var row = table.NewRow();
			row[resultColumnName] = resultKey;
			foreach (var (key, value) in results)
			{
				row[key] = value;
			}
			table.Rows.Add(row);
		}
		return table;
	}
}
