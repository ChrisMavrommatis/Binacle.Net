using Binacle.CompactNotation;
using Binacle.Net.UIModule.ViewModels;
using Microsoft.AspNetCore.Hosting;

namespace Binacle.Net.UIModule.Services;

internal interface ISampleDataService
{
	ViewModels.PackingDemoViewModel GetSampleData(int? binsIndex = null, int? itemsIndex = null);
}

internal class SampleDataService : ISampleDataService
{
	private class SampleJsonData
	{
		public Dictionary<string, List<string>>? BinSets { get; set; }
		public Dictionary<string, List<string>>? ItemSets { get; set; }
	}

	private readonly IWebHostEnvironment environment;
	private readonly TimeProvider timeProvider;
	private readonly SampleJsonData data;

	public SampleDataService(
		IWebHostEnvironment environment,
		TimeProvider timeProvider
		)
	{
		this.environment = environment;
		this.timeProvider = timeProvider;
		this.data = this.ReadSampleData();
	}

	public ViewModels.PackingDemoViewModel GetSampleData(int? binsIndex = null, int? itemsIndex = null)
	{
		var actualBinsIndex = binsIndex;
		var actualItemsIndex = itemsIndex;

		if (actualBinsIndex is null || actualItemsIndex is null)
		{
			var random = new Random(this.timeProvider.GetUtcNow().Millisecond);
			if (actualBinsIndex is null)
			{
				actualBinsIndex = random.Next(this.data.BinSets!.Count);
			}

			if (actualItemsIndex is null)
			{
				actualItemsIndex = random.Next(this.data.ItemSets!.Count);
			}
		}

		var bins = this.data.BinSets!
			.ElementAt(actualBinsIndex.Value)
			.Value;

		var items = this.data.ItemSets!
			.ElementAt(actualItemsIndex.Value)
			.Value;

		return new ViewModels.PackingDemoViewModel
		{
			Algorithm = Algorithm.FirstFitDecreasing,
			Bins = ParseBins(bins),
			Items = ParseItems(items)
		};
	}

	private SampleJsonData ReadSampleData()
	{
		var fileInfo = this.environment.WebRootFileProvider.GetFileInfo("data/sample_data.json");

		if (!fileInfo.Exists)
		{
			return _defaultJsonSampleData;
		}

		var json = System.IO.File.ReadAllText(fileInfo.PhysicalPath!);
		var sampleJsonData = System.Text.Json.JsonSerializer.Deserialize<SampleJsonData>(json)!;

		return sampleJsonData;
	}

	private List<ViewModels.Bin> ParseBins(List<string> bins)
	{
		return bins.Select(ParseBin).ToList();
	}

	private ViewModels.Bin ParseBin(string value)
	{
		var dimensions = CompactNotationParser.ParseDimensions<int>(value);
		return new ViewModels.Bin(dimensions.Length, dimensions.Width, dimensions.Height);
	}

	private List<ViewModels.Item> ParseItems(List<string> items)
	{
		return items.Select(ParseItem).ToList();
	}

	private ViewModels.Item ParseItem(string value)
	{
		var parsed = CompactNotationParser.ParseDimensionsAndQuantity<int>(value);
		return new ViewModels.Item(parsed.Length, parsed.Width, parsed.Height, parsed.Quantity);
	}

	private static SampleJsonData _defaultJsonSampleData = new SampleJsonData()
	{
		BinSets = new Dictionary<string, List<string>>()
		{
			{"Default Sample Bin Set 1", ["60x40x10"] },
		},
		ItemSets = new Dictionary<string, List<string>>()
		{
			{"Default Sample Item Set 1", ["2x5x10 [7]","12x15x10 [3]", "10x15x15 [2]" ] }
		}
	};
}
