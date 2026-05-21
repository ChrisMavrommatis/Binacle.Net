using System.Data;
using System.Numerics;

namespace Binacle.Lib.PerformanceTests.Results;

internal class CollectionStatisticsResult<T> : Dictionary<string, List<T>>, IResult
	where T: struct,  INumber<T>, IComparable<T>
{
	private readonly string columnName;
	private readonly string unit;

	public CollectionStatisticsResult(string columnName, string unit) : base()
	{
		this.columnName = columnName;
		this.unit = unit;
	}
	public void AddValue(string collectionName, T value)
	{
		if (!this.TryGetValue(collectionName, out var result))
		{
			result = new List<T>();
			this.Add(collectionName, result);
		}
		result.Add(value);
	}

	public DataTable ToDataTable()
	{
		var table = new DataTable();
		table.Columns.Add(this.columnName, typeof(string));
		table.Columns.Add($"Min ({this.unit})", typeof(T));
		table.Columns.Add($"Max ({this.unit})", typeof(T));
		table.Columns.Add($"Mean ({this.unit})", typeof(double));
		table.Columns.Add($"Median ({this.unit})", typeof(double));
		table.Columns.Add($"StdDev ({this.unit})", typeof(double));
		
			
		foreach(var (collectionKey, values) in this)
		{
			var orderedValues = values.OrderBy(x => x).ToList();
			var mean = Math.Round((orderedValues.Average(x => Convert.ToDouble(x))), 2);
			var row = table.NewRow();
			row[this.columnName] = collectionKey;
			row[$"Min ({this.unit})"] = orderedValues.Min();
			row[$"Max ({this.unit})"] = orderedValues.Max();
			row[$"Mean ({this.unit})"] = mean; 
			row[$"Median ({this.unit})"] = Math.Round(GetMedian(orderedValues), 2);
			row[$"StdDev ({this.unit})"] = Math.Round(GetStdDev(orderedValues, mean), 2);
			
			table.Rows.Add(row);
		}
		
		return table;
	}

	private double GetStdDev(List<T> orderedValues, double mean)
	{
		if (orderedValues.Count < 2)
		{
			return 0;
		}
        
		var sumSquaredDiffs = orderedValues
			.Select(val => Convert.ToDouble(val))
			.Sum(value => Math.Pow(value - mean, 2));
        
		return Math.Sqrt(sumSquaredDiffs / orderedValues.Count);
	}

	private double GetMedian(List<T> orderedValues)
	{
		int count = orderedValues.Count;
		if (count % 2 == 0)
		{
			// Even number of elements
			var middle1 = Convert.ToDouble(orderedValues[(count / 2) - 1]);
			var middle2 = Convert.ToDouble(orderedValues[count / 2]);
			return (middle1 + middle2) / 2.0;
		}
		else
		{
			// Odd number of elements
			return Convert.ToDouble(orderedValues[count / 2]);
		}
	}
}
