using Binacle.Lib.Components.Models;
using Binacle.Lib.Tests.Models;

namespace Binacle.Lib.Tests
{
    public class BinacleStrategiesFixture : IDisposable
    {
        public readonly List<Item> GlockersBins;

        public BinacleStrategiesFixture()
        {
            this.GlockersBins = new List<Item>()
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
