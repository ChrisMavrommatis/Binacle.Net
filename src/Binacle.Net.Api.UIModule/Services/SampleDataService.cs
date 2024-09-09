using Microsoft.AspNetCore.Hosting;

namespace Binacle.Net.Api.UIModule.Services;

public interface ISampleDataService
{
	Models.BinPackingViewModel GetRandomSampleData();
	Models.BinPackingViewModel GetInitialSampleData();
}

public class SampleDataService : ISampleDataService
{
	private class SampleJsonData
	{
		public Dictionary<string, List<string>> BinSets { get; set; }
		public Dictionary<string, List<string>> ItemSets { get; set; }
	}

	private class SampleData
	{
		public List<List<Models.Bin>> BinSets { get; set; }
		public List<List<Models.Item>> ItemSets { get; set; }
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

	public Models.BinPackingViewModel GetRandomSampleData()
	{
		// random with time
		var random = new Random(this.timeProvider.GetUtcNow().Millisecond);

		// random bin and Items
		var binRandomNumber = random.Next(this.data.BinSets.Count);
		var itemRandomNumber = random.Next(this.data.ItemSets.Count);

		var bins = this.data.BinSets[binRandomNumber];
		var items = this.data.ItemSets[itemRandomNumber];

		return new Models.BinPackingViewModel
		{
			Bins = bins,
			Items = items
		};
	}

	public Models.BinPackingViewModel GetInitialSampleData()
	{
		var bins = this.data.BinSets[0];
		var items = this.data.ItemSets[0];

		return new Models.BinPackingViewModel
		{
			Bins = bins,
			Items = items
		};
	}

	private SampleData ReadSampleData()
	{
		var fileInfo = this.environment.WebRootFileProvider.GetFileInfo("data/sample_data.json");
		var sampleData = new SampleData();
		if (!fileInfo.Exists)
		{
			return sampleData;
		}

		var json = System.IO.File.ReadAllText(fileInfo.PhysicalPath!);
		var sampleJsonData = System.Text.Json.JsonSerializer.Deserialize<SampleJsonData>(json)!;

		sampleData.BinSets = sampleJsonData.BinSets.Values.Select(binSet => binSet.Select(ParseBin).ToList()).ToList();
		sampleData.ItemSets = sampleJsonData.ItemSets.Values.Select(itemSet => itemSet.Select(ParseItem).ToList()).ToList();
		return sampleData;
	}

	private Models.Bin ParseBin(string value)
	{
		var dimensionParts = value.Split('x');
		if (dimensionParts.Length != 3)
		{
			throw new InvalidOperationException("Invalid bin data");
		}
		return new Models.Bin(
			int.Parse(dimensionParts[0]),
			int.Parse(dimensionParts[1]),
			int.Parse(dimensionParts[2])
		);
	}

	private Models.Item ParseItem(string value)
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

		return new Models.Item(
			int.Parse(dimensions[0]),
			int.Parse(dimensions[1]),
			int.Parse(dimensions[2]),
			quantity
		);
	}
}
