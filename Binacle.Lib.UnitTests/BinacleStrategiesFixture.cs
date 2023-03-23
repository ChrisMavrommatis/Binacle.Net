using Binacle.Lib.Components.Models;

namespace Binacle.Lib.Tests
{
    public class BinacleStrategiesFixture : IDisposable
    {
        public readonly List<Bin> GlockersBins;

        public BinacleStrategiesFixture()
        {
            this.GlockersBins = new List<Bin>()
            {
                new Bin("Small", new Dimensions(8,45,62)),
                new Bin("Medium", new Dimensions(17,45,62)),
                new Bin("Large", new Dimensions(36,45,62))
            };

        }

        public void Dispose()
        {
            
        }
    }
}
