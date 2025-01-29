using System.Buffers.Text;
using System.Text;
using Binacle.PackingVisualizationProtocol.Models;

namespace Binacle.PackingVisualizationProtocol.Tests;

public static class Program
{
	public static async Task Main(string[] args)
	{
		Console.WriteLine("Yes");

		var bin = new Bin<ushort>()
		{
			Length = 60,
			Width = 40,
			Height = 30,
		};

		List<Item<ushort>> items =
		[
			new () { Length = 12, Width = 15, Height = 10, X = 0, Y = 0, Z = 0 },
			new () { Length = 12, Width = 10, Height = 15, X = 12, Y = 0, Z = 0 },
			new () { Length = 2, Width = 5, Height = 10, X = 0, Y = 15, Z = 0 },
			new () { Length = 2, Width = 5, Height = 10, X = 0, Y = 0, Z = 10 },
		];

		var bytes = PackingVisualizationProtocolSerializer.SerializeUInt16(bin, items);
		
		var encoded  = System.Convert.ToBase64String(bytes);
		
		Console.WriteLine(encoded);


		var compressed1 = "H4sIAAAAAAAAA1XKQQqAMBQD0UexGxG68QzSQ3j/c5WAH+psEiZ5DjcmXlwYOP2Jj8sWKhv6t+VT7L5IX2YJrGRvAAAA";
		var compressed2 = "H4sIAAAAAAAAA1NjYeBiYGDQYGBgsGFgALN5GBgY+BlQAbo4iA0CTAwMDKxQeRgAqUEXl4BKAgAtO/H2bwAAAA==";
		
		var decoded1 = System.Convert.FromBase64String(compressed1);
		var decoded2 = System.Convert.FromBase64String(compressed2);
		
		var result1 = PackingVisualizationProtocolSerializer.DeserializeInt32<Bin<int>, Item<int>>(decoded1);
		var result2 = PackingVisualizationProtocolSerializer.DeserializeInt32<Bin<int>, Item<int>>(decoded2);
		
		Console.WriteLine(result1);
		Console.WriteLine(result2);

	}
}
