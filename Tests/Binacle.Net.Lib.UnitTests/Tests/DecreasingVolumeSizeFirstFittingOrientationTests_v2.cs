using AutoFixture;
using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Exceptions;
using Binacle.Net.Lib.Models;
using Binacle.Net.Lib.Tests.Models;
using Xunit;

namespace Binacle.Net.Lib.Tests
{
    public class DecreasingVolumeSizeFirstFittingOrientationTests_v2 : IClassFixture<BinacleStrategiesFixture>
    {
        private BinacleStrategiesFixture Fixture { get; }
        public Fixture AutoFixture { get; }

        public DecreasingVolumeSizeFirstFittingOrientationTests_v2(BinacleStrategiesFixture fixture)
        {
            this.Fixture = fixture;
            this.AutoFixture = new AutoFixture.Fixture();
            //this.AutoFixture.Customize<IWithReadOnlyDimensions<int>>(x => x.FromFactory(() => new TestItemWithDimensions()));
        }

        [Fact]
        public void TestsWork()
        {

        }

        [Fact]
        public void WithBinsAndItems_NullOrEmpty_OnBuildThrows_ArgumentNullException()
        {
            var testItems = this.AutoFixture.CreateMany<TestItem>(2);
            var testBins = this.AutoFixture.CreateMany<TestBin>(2);

            Assert.Throws<ArgumentNullException>(() =>
            {
                var strategy = new Binacle.Net.Lib.Strategies.DecreasingVolumeSize_v2()
                .WithBins((IEnumerable<TestBin>)null)
                .AndItems(testItems)
                .Build();
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                var strategy = new Binacle.Net.Lib.Strategies.DecreasingVolumeSize_v2()
                .WithBins(new List<TestBin>())
                .AndItems(testItems)
                .Build();
            });


            Assert.Throws<ArgumentNullException>(() =>
            {
                var strategy = new Binacle.Net.Lib.Strategies.DecreasingVolumeSize_v2()
                .WithBins(testBins)
                .AndItems((IEnumerable<TestItem>)null)
                .Build();
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                var strategy = new Binacle.Net.Lib.Strategies.DecreasingVolumeSize_v2()
                .WithBins(testBins)
                .AndItems(new List<TestItem>())
                .Build();
            });
        }

        [Fact]
        public void With0DimensionBinsAndItems_OnBuildThrows_DimensionException()
        {
            var testItems = this.AutoFixture.CreateMany<TestItem>(2);
            var testItemsWith0Dimension = this.AutoFixture.Build<TestItem>()
                .With(x => x.Width, 0)
                .CreateMany(2);

            var testBins = this.AutoFixture.CreateMany<TestBin>(2);
            var testBinsWith0Dimension = this.AutoFixture.Build<TestBin>()
                .With(x => x.Width, 0)
                .CreateMany(2);

            Assert.Throws<DimensionException>(() =>
            {
                var strategy = new Binacle.Net.Lib.Strategies.DecreasingVolumeSize_v2()
                .WithBins(testBinsWith0Dimension)
                .AndItems(testItems)
                .Build();
            });

            var bins = testItems.Select(x => new Item(x.ID, x)).ToList();
            var itemsWith0Dimension = testItemsWith0Dimension.Select(x => new Item(x.ID, x)).ToList();

            Assert.Throws<DimensionException>(() =>
            {
                var strategy = new Binacle.Net.Lib.Strategies.DecreasingVolumeSize_v2()
                .WithBins(testBins)
                .AndItems(testItemsWith0Dimension)
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
            var _5x5x5 = new Dimensions<int>(5, 5, 5);
            var _items_x_5x5x5 = Enumerable.Range(1, x).Select(x => new TestItem(x.ToString(), _5x5x5)).ToList();

            var strategy = new Binacle.Net.Lib.Strategies.DecreasingVolumeSize_v2()
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
