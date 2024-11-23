using System.Data;
using System.Numerics;
using ConsoleTables;

namespace Binacle.Net.Lib.PerformanceTests.Models;

internal class ComparisonResults<T> : IResult
	where T : struct,  INumber<T>, IComparable<T>
{
	private Dictionary<string, Dictionary<string, T>> dictionary;

	public ComparisonResults()
	{
		this.dictionary = new Dictionary<string, Dictionary<string, T>>();
	}
	
	public void Add(string scenario, string algorithm, T value)
	{
		if (!this.dictionary.TryGetValue(scenario, out var discrepancy))
		{
			discrepancy = new Dictionary<string, T>();
			this.dictionary[scenario] = discrepancy;
		}
		discrepancy[algorithm] = value;
	}
	
	public required string Unit { get; init; }

	public DataTable ToDataTable()
	{
		var table = new DataTable();
		table.Columns.Add("Scenario", typeof(string));
		
		foreach (var algorithm in this.dictionary.Values.SelectMany(x => x.Keys).Distinct())
		{
			table.Columns.Add($"{algorithm} ({this.Unit})", typeof(T));
		}

		foreach (var (scenario, algorithmResults) in this.dictionary)
		{
			var noOfValues = algorithmResults.Count + 1;
			var values = new object[noOfValues];
			values[0] = scenario;
			var i = 1;
			foreach (var algorithm in algorithmResults.Keys)
			{
				values[i] = algorithmResults[algorithm];
				i++;
			}
			
			table.Rows.Add(values);
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
