using Binacle.TestsKernel.Models;

namespace Binacle.Lib.Benchmarks;

public static class Generator
{
	public static Random Random = new Random(8875223);
	
	public static List<TestBin> GenerateBins(int length, int width, int height, int count)
	{
		var bins = new List<TestBin>(count);
		for (int i = 0; i < count; i++)
		{
			bins.Add(new TestBin
			{
				ID = $"{length}x{width}x{height}",
				Length = length,
				Width = width,
				Height = height
			});
		}
		return bins;
	}
	
	public static List<TestBin> GenerateBins(int count, int mixSize, int maxSize)
	{
		var bins = new List<TestBin>(count);
		for (int i = 0; i < count; i++)
		{
			var length = Random.Next(mixSize, maxSize + 1);
			var width = Random.Next(mixSize, maxSize + 1);
			var height = Random.Next(mixSize, maxSize + 1);
			bins.Add(new TestBin
			{
				ID = $"{length}x{width}x{height}",
				Length = length,
				Width = width,
				Height = height
			});
			
		}

		return bins;
	}
	
	public static List<TestItem> GenerateItems(int count, int minSize, int maxSize)
	{
		var items = new List<TestItem>();
		var itemsGenerated = 0;
		while (itemsGenerated < count)
		{
			var quantity = Random.Next(1, 5); // 1-4 units per SKU
			var remaining = count - itemsGenerated;
			quantity = Math.Min(quantity, remaining); // Don't exceed total count
			var length = Random.Next(minSize, maxSize + 1);
			var width = Random.Next(minSize, maxSize + 1);
			var height = Random.Next(minSize, maxSize + 1);
			items.Add(new TestItem
			{
				ID = $"{length}x{width}x{height}-{quantity}",
				Length = length,
				Width = width,
				Height = height,
				Quantity = quantity,
			});
			itemsGenerated+= quantity;
		}
    
		return items;
	}
	
}
