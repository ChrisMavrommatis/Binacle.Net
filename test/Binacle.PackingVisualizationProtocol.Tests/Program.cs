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

		var bytes = PackingVisualizationProtocolSerializer.Serialize(bin, items);
		
		var encoded  = System.Convert.ToBase64String(bytes);
		
		Console.WriteLine(encoded);
		
	}
}
