using System.Numerics;

namespace Binacle.Net.Lib.PerformanceTests.Models;

internal class AlgorithmResults<T> : IEnumerable<KeyValuePair<string, List<T>>>
	where T :  struct, INumber<T>, IComparable<T>
{
	private readonly Dictionary<string, List<T>> dictionary;
	public AlgorithmResults()
	{
		this.dictionary = new Dictionary<string, List<T>>();
	}
	public void AddValue(string algorithmName, T value)
	{
		if (!this.dictionary.TryGetValue(algorithmName, out var result))
		{
			result = new List<T>();
			this.dictionary.Add(algorithmName, result);
		}
		result.Add(value);
	}

	public MeasurementResults<T> GetMeasurementResults(string unit)
	{
		var measurementResults = new MeasurementResults<T>()
		{
			Unit = unit
		};

		foreach (var (key, _) in this)
		{
			measurementResults.Add(new MeasurementResult<T>()
			{
				Algorithm = key,
				Min = this.GetMin(key),
				Max = this.GetMax(key),
				Mean = this.GetMean(key),
				Median = this.GetMedian(key)
			});

		}

		return measurementResults;
	}

	public T GetMin(string algorithmName)
	{
		return this.GetResults(algorithmName).Min()!;	
	}

	public T GetMax(string algorithmName)
	{
		return this.GetResults(algorithmName).Max()!;	
	} 
	
	public double GetMean(string algorithmName)
	{
		var results = this.GetResults(algorithmName);

		if (!results.Any())
		{
			throw new InvalidOperationException("No results available for the given algorithm name.");
		}

		double average = results switch
		{
			IEnumerable<int> intResults => intResults.Average(),
			IEnumerable<float> floatResults => floatResults.Average(),
			IEnumerable<double> doubleResults => doubleResults.Average(),
			_ => throw new NotSupportedException($"Type {typeof(T)} is not supported.")
		};

		return Math.Round(average, 2, MidpointRounding.AwayFromZero);
	}

	public double GetMedian(string algorithmName)
	{
		var results = GetResults(algorithmName).OrderBy(x => x).ToList();
		int count = results.Count;
		if (count == 0)
		{
			throw new InvalidOperationException("No results available.");
		}
		
		var medianIndex = count / 2;
		
		if(count % 2 == 1)
		{
			var evenMedianValue = Convert.ToDouble(results[medianIndex]);
			return Math.Round(evenMedianValue, 2, MidpointRounding.AwayFromZero);
		}
		
		var oddMedianValue = (Convert.ToDouble(results[medianIndex - 1]) + Convert.ToDouble(results[medianIndex])) / 2;
		
		return Math.Round(oddMedianValue, 2, MidpointRounding.AwayFromZero);
	}

	private List<T> GetResults(string algorithmName)
	{
		if(!this.dictionary.TryGetValue(algorithmName, out var results))
		{
			throw new KeyNotFoundException("No results for the given algorithm name.");
		}
		return results;		
	}
	
	public IEnumerator<KeyValuePair<string, List<T>>> GetEnumerator()
	{
		return this.dictionary.GetEnumerator();
	}

	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}
}
