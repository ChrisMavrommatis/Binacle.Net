using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Strategies.Models;

namespace Binacle.Net.Lib.Strategies
{
    internal sealed partial class DecreasingVolumeSize_v1
    {
        private sealed class Bin : BinBase
        {
            public Bin(string id, IWithReadOnlyDimensions<ushort> item) : base(id, item)
            {
            }
            public Bin(string id, ushort length, ushort width, ushort height) : base(id, length, width, height)
            {

            }
        }
    }
}
