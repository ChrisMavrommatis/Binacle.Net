using Binacle.TestsKernel.Models;

namespace Binacle.Lib.Benchmarks;

public class Generator
{
	private readonly Random random;

	public Generator(int seed)
	{
		this.random = new Random(seed);
		
	}
	public List<TestBin> GenerateBins(int count, int length, int width, int height)
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
	
	public List<TestBin> GenerateBins(int count, int minSize, int maxSize)
	{
		var bins = new List<TestBin>(count);
		for (int i = 0; i < count; i++)
		{
			var length = this.random.Next(minSize, maxSize + 1);
			var width = this.random.Next(minSize, maxSize + 1);
			var height = this.random.Next(minSize, maxSize + 1);
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
	
	public List<TestItem> GenerateItems(int count, int minSize, int maxSize)
	{
		var items = new List<TestItem>();
		var itemsGenerated = 0;
		while (itemsGenerated < count)
		{
			var quantity = this.random.Next(1, 5); // 1-4 units per SKU
			var remaining = count - itemsGenerated;
			quantity = Math.Min(quantity, remaining); // Don't exceed total count
			var length = this.random.Next(minSize, maxSize + 1);
			var width = this.random.Next(minSize, maxSize + 1);
			var height = this.random.Next(minSize, maxSize + 1);
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
