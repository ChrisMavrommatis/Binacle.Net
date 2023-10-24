using Binacle.Net.Lib.Models;
using Binacle.Net.Lib.Tests.Models;

namespace Binacle.Net.Lib.Tests
{
    public class BinacleStrategiesFixture : IDisposable
    {
        public readonly List<Item> Bins;

        public BinacleStrategiesFixture()
        {
            this.Bins = new List<Item>()
            {
                new Item("Small", new Dimensions<ushort>(8,45,62)),
                new Item("Medium", new Dimensions<ushort>(17,45,62)),
                new Item("Large", new Dimensions<ushort>(36,45,62))
            };

        }

        public void Dispose()
        {
            
        }
    }
}
