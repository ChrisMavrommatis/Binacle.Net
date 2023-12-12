using Binacle.Net.Lib.Models;
using Binacle.Net.Lib.Tests.Models;

namespace Binacle.Net.Lib.Tests
{
    public class BinacleStrategiesFixture : IDisposable
    {
        public readonly List<TestBin> Bins;

        public BinacleStrategiesFixture()
        {
            this.Bins = new List<TestBin>()
            {
                new TestBin("Small", new Dimensions<int>(8,45,62)),
                new TestBin("Medium", new Dimensions<int>(17,45,62)),
                new TestBin("Large", new Dimensions<int>(36,45,62))
            };

        }

        public void Dispose()
        {
            
        }
    }
}
