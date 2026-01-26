using Binacle.TestsKernel.Helpers;

namespace Binacle.TestsKernel.Models;

public class Scenario
{
	public required string Name { get; init; }
	public required TestBin Bin { get; init; }
	public required List<TestItem> Items { get; init; }
	public override string ToString() => Name;


	public static Scenario Create(string name, string bin, string[] items, string result)
	{
		var parsedBinDimensions = DimensionsHelper.ParseFromCompactString(bin);
		var parsedItems = items.Select(x =>
		{
			var dimensions = DimensionsHelper.ParseFromCompactString(x);
			return new TestItem(x, dimensions, dimensions.Quantity);
		}).ToList();
		return new Scenario
		{
			Name = name,
			Bin = new TestBin(bin, parsedBinDimensions),
			Items = parsedItems
		};
	}
}
