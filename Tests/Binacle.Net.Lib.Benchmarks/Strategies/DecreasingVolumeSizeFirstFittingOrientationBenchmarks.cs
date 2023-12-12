using BenchmarkDotNet.Attributes;
using Binacle.Net.Lib.Benchmarks.Models;
using Binacle.Net.Lib.Models;

namespace Binacle.Net.Lib.Benchmarks.Strategies
{
    [RankColumn]
    [MemoryDiagnoser]
    //[Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.SlowestToFastest)]
    public class DecreasingVolumeSizeFirstFittingOrientationBenchmarks
    {
        private readonly StrategyFactory strategyFactory;
        private readonly List<TestItem> items_10_5x5x5;
        private readonly List<TestItem> items_50_5x5x5;
        private readonly List<TestItem> items_100_5x5x5;
        private readonly List<TestBin> bins;

        public DecreasingVolumeSizeFirstFittingOrientationBenchmarks()
        {
            this.strategyFactory = new Binacle.Net.Lib.StrategyFactory();

            var _5x5x5 = new Dimensions<int>(5, 5, 5);

            this.items_10_5x5x5 = Enumerable.Range(1, 10).Select(x => new TestItem(x.ToString(), _5x5x5)).ToList();
            this.items_50_5x5x5 = Enumerable.Range(1, 50).Select(x => new TestItem(x.ToString(), _5x5x5)).ToList();
            this.items_100_5x5x5 = Enumerable.Range(1, 100).Select(x => new TestItem(x.ToString(), _5x5x5)).ToList();

            this.bins = new List<TestBin>()
            {
                new TestBin("Small", new Dimensions<int>(8,45,62)),
                new TestBin("Medium", new Dimensions<int>(17,45,62)),
                new TestBin("Large", new Dimensions<int>(36,45,62))
            };
        }

        [Benchmark]
        public void With_10_5x5x5_v1()
        {
            var strategy = new Lib.Strategies.DecreasingVolumeSize_v1()
                .WithBins(this.bins)
                .AndItems(this.items_10_5x5x5)
                .Build();

            var result = strategy.Execute();
        }

        [Benchmark]
        public void With_10_5x5x5_v2()
        {
            var strategy = new Lib.Strategies.DecreasingVolumeSize_v2()
                .WithBins(this.bins)
                .AndItems(this.items_10_5x5x5)
                .Build();

            var result = strategy.Execute();
        }


        [Benchmark]
        public void With_50_5x5x5_v1()
        {
            var strategy = new Lib.Strategies.DecreasingVolumeSize_v1()
                .WithBins(this.bins)
                .AndItems(this.items_50_5x5x5)
                .Build();

            var result = strategy.Execute();
        }


        [Benchmark]
        public void With_50_5x5x5_v2()
        {
            var strategy = new Lib.Strategies.DecreasingVolumeSize_v2()
                .WithBins(this.bins)
                .AndItems(this.items_50_5x5x5)
                .Build();

            var result = strategy.Execute();
        }

        [Benchmark]
        public void With_100_5x5x5_v1()
        {
            var strategy = new Lib.Strategies.DecreasingVolumeSize_v1()
                .WithBins(this.bins)
                .AndItems(this.items_100_5x5x5)
                .Build();

            var result = strategy.Execute();
        }


        [Benchmark]
        public void With_100_5x5x5_v2()
        {
            var strategy = new Lib.Strategies.DecreasingVolumeSize_v2()
                .WithBins(this.bins)
                .AndItems(this.items_100_5x5x5)
                .Build();

            var result = strategy.Execute();
        }
    }
}
