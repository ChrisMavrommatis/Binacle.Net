using Binacle.Net.Api.UIModule.ViewModels;
using Microsoft.AspNetCore.Hosting;

namespace Binacle.Net.Api.UIModule.Services;

internal interface ISampleDataService
{
	ViewModels.BinPackingViewModel GetRandomSampleData();
	ViewModels.BinPackingViewModel GetInitialSampleData();
}

internal class SampleDataService : ISampleDataService
{
	private class SampleJsonData
	{
		public Dictionary<string, List<string>>? BinSets { get; set; }
		public Dictionary<string, List<string>>? ItemSets { get; set; }
	}

	private class SampleData
	{
		public required List<List<ViewModels.Bin>> BinSets { get; set; }
		public required List<List<ViewModels.Item>> ItemSets { get; set; }
	}

	private readonly IWebHostEnvironment environment;
	private readonly TimeProvider timeProvider;
	private readonly SampleData data;

	public SampleDataService(
		IWebHostEnvironment environment,
		TimeProvider timeProvider
		)
	{
		this.environment = environment;
		this.timeProvider = timeProvider;
		this.data = this.ReadSampleData();
	}

	public ViewModels.BinPackingViewModel GetRandomSampleData()
	{
		// random with time
		var random = new Random(this.timeProvider.GetUtcNow().Millisecond);

		// random bin and Items
		var binRandomNumber = random.Next(this.data.BinSets.Count);
		var itemRandomNumber = random.Next(this.data.ItemSets.Count);

		var bins = this.data.BinSets[binRandomNumber];
		var items = this.data.ItemSets[itemRandomNumber];

		return new ViewModels.BinPackingViewModel
		{
			Algorithm = Algorithm.FirstFitDecreasing,
			Bins = bins,
			Items = items
		};
	}

	public ViewModels.BinPackingViewModel GetInitialSampleData()
	{
		var bins = this.data.BinSets[0];
		var items = this.data.ItemSets[0];

		return new ViewModels.BinPackingViewModel
		{
			Algorithm = Algorithm.FirstFitDecreasing,
			Bins = bins,
			Items = items
		};
	}

	private SampleData ReadSampleData()
	{
		var fileInfo = this.environment.WebRootFileProvider.GetFileInfo("data/sample_data.json");

		if (!fileInfo.Exists)
		{
			return new SampleData()
			{
				BinSets = new List<List<ViewModels.Bin>>(),
				ItemSets = new List<List<ViewModels.Item>>()
			};
		}

		var json = System.IO.File.ReadAllText(fileInfo.PhysicalPath!);
		var sampleJsonData = System.Text.Json.JsonSerializer.Deserialize<SampleJsonData>(json)!;

		var sampleData = new SampleData
		{
			BinSets = sampleJsonData.BinSets?.Values.Select(binSet => binSet.Select(ParseBin).ToList()).ToList() ?? new List<List<ViewModels.Bin>>(),
			ItemSets = sampleJsonData.ItemSets?.Values.Select(itemSet => itemSet.Select(ParseItem).ToList()).ToList() ?? new List<List<ViewModels.Item>>()
		};
		return sampleData;
	}

	private ViewModels.Bin ParseBin(string value)
	{
		var dimensionParts = value.Split('x');
		if (dimensionParts.Length != 3)
		{
			throw new InvalidOperationException("Invalid bin data");
		}
		return new ViewModels.Bin(
			int.Parse(dimensionParts[0]),
			int.Parse(dimensionParts[1]),
			int.Parse(dimensionParts[2])
		);
	}

	private ViewModels.Item ParseItem(string value)
	{
		var dimensionsAndQuantityParts = value.Split('-');

		if (dimensionsAndQuantityParts.Length > 2)
		{
			throw new InvalidOperationException("Invalid item data");
		}
		var dimensions = dimensionsAndQuantityParts[0].Split('x');

		if (dimensions.Length != 3)
		{
			throw new InvalidOperationException("Invalid item data");
		}

		var quantity = 1;
		if (dimensionsAndQuantityParts.Length == 2)
		{
			quantity = int.Parse(dimensionsAndQuantityParts[1]);
		}

		return new ViewModels.Item(
			int.Parse(dimensions[0]),
			int.Parse(dimensions[1]),
			int.Parse(dimensions[2]),
			quantity
		);
	}
}
