using AutoFixture;
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Exceptions;
using Binacle.Lib.Models;
using Binacle.Lib.Tests.Models;
using Xunit;

namespace Binacle.Lib.Tests
{
    public class DecreasingVolumeSizeFirstFittingOrientationTests_v2 : IClassFixture<BinacleStrategiesFixture>
    {
        private BinacleStrategiesFixture Fixture { get; }
        public Fixture AutoFixture { get; }

        public DecreasingVolumeSizeFirstFittingOrientationTests_v2(BinacleStrategiesFixture fixture)
        {
            this.Fixture = fixture;
            this.AutoFixture = new AutoFixture.Fixture();
            this.AutoFixture.Customize<IWithReadOnlyDimensions<ushort>>(x => x.FromFactory(() => new TestItemWithDimensions()));
        }

        [Fact]
        public void TestsWork()
        {

        }

        [Fact]
        public void WithBinsAndItems_NullOrEmpty_OnBuildThrows_ArgumentNullException()
        {
            var testItems = this.AutoFixture.CreateMany<TestItemWithDimensions>(2);
            var items = testItems.Select(x => new Item(x.ID, x)).ToList();

            Assert.Throws<ArgumentNullException>(() =>
            {
                var strategy = new Binacle.Lib.Strategies.DecreasingVolumeSize_v2()
                .WithBins(null)
                .AndItems(items)
                .Build();
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                var strategy = new Binacle.Lib.Strategies.DecreasingVolumeSize_v2()
                .WithBins(new List<Item>())
                .AndItems(items)
                .Build();
            });

            var bins = testItems.Select(x => new Item(x.ID, x)).ToList();

            Assert.Throws<ArgumentNullException>(() =>
            {
                var strategy = new Binacle.Lib.Strategies.DecreasingVolumeSize_v2()
                .WithBins(bins)
                .AndItems(null)
                .Build();
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                var strategy = new Binacle.Lib.Strategies.DecreasingVolumeSize_v2()
                .WithBins(bins)
                .AndItems(new List<Item>())
                .Build();
            });
        }

        [Fact]
        public void With0DimensionBinsAndItems_OnBuildThrows_DimensionException()
        {
            var testItems = this.AutoFixture.CreateMany<TestItemWithDimensions>(2);
            var testItemsWith0Dimension = this.AutoFixture.Build<TestItemWithDimensions>()
                .With(x => x.Width, 0)
                .CreateMany(2);

            var binsWith0Dimension = testItemsWith0Dimension.Select(x => new Item(x.ID, x)).ToList();
            var items = testItems.Select(x => new Item(x.ID, x)).ToList();

            Assert.Throws<DimensionException>(() =>
            {
                var strategy = new Binacle.Lib.Strategies.DecreasingVolumeSize_v2()
                .WithBins(binsWith0Dimension)
                .AndItems(items)
                .Build();
            });

            var bins = testItems.Select(x => new Item(x.ID, x)).ToList();
            var itemsWith0Dimension = testItemsWith0Dimension.Select(x => new Item(x.ID, x)).ToList();

            Assert.Throws<DimensionException>(() =>
            {
                var strategy = new Binacle.Lib.Strategies.DecreasingVolumeSize_v2()
                .WithBins(bins)
                .AndItems(itemsWith0Dimension)
                .Build();
            });
        }

        [Theory]
        [InlineData(10, "Small")]
        [InlineData(50, "Small")]
        [InlineData(100, "Small")]
        [InlineData(110, "Medium")]
        public void Glockers_Baseline_X_5x5x5_FitOn_Y(int x, string y)
        {
            var _5x5x5 = new Dimensions<ushort>(5, 5, 5);
            var _items_x_5x5x5 = Enumerable.Range(1, x).Select(x => new Item(x.ToString(), _5x5x5)).ToList();

            var strategy = new Binacle.Lib.Strategies.DecreasingVolumeSize_v2()
                .WithBins(this.Fixture.Bins)
                .AndItems(_items_x_5x5x5)
                .Build();

            var result = strategy.Execute();

            Assert.NotNull(result);
            Assert.NotNull(result.FoundBin);
            Assert.Equal(y, result.FoundBin.ID);
        }


    }

}
