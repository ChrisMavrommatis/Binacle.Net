using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Strategies.Models;

namespace Binacle.Net.Lib.Fitting.Algorithms;

internal sealed partial class FirstFitDecreasing_v1
{
	private sealed class Bin : BinBase
	{
		public Bin(string id, IWithReadOnlyDimensions<int> item) : base(id, item)
		{
		}
		public Bin(string id, int length, int width, int height) : base(id, length, width, height)
		{

		}
	}
}
