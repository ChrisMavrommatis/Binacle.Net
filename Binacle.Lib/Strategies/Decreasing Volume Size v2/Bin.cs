using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Strategies.Models;

namespace Binacle.Lib.Strategies
{
    internal sealed partial class DecreasingVolumeSize_v2
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
