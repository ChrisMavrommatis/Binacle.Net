using AutoFixture;
using Binacle.Lib.Components.Exceptions;
using Binacle.Lib.Components.Models;
using Binacle.Lib.Tests.Models;
using Xunit;

namespace Binacle.Lib.Tests
{
    public class DecreasingVolumeSizeFirstFittingOrientationTests : IClassFixture<BinacleStrategiesFixture>
    {
        private BinacleStrategiesFixture Fixture { get; }
        public Fixture AutoFixture { get; }

        public DecreasingVolumeSizeFirstFittingOrientationTests(BinacleStrategiesFixture fixture)
        {
            this.Fixture = fixture;
            this.AutoFixture = new AutoFixture.Fixture();
            this.AutoFixture.Customize<IWithDimensions>(x => x.FromFactory(() => new TestItemWithDimensions()));
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
                var strategy = new Binacle.Lib.Strategies.DecreasingVolumeSizeFirstFittingOrientation()
                .WithBins(null)
                .AndItems(items)
                .Build();
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                var strategy = new Binacle.Lib.Strategies.DecreasingVolumeSizeFirstFittingOrientation()
                .WithBins(new List<Bin>())
                .AndItems(items)
                .Build();
            });

            var bins = testItems.Select(x => new Bin(x.ID, x)).ToList();

            Assert.Throws<ArgumentNullException>(() =>
            {
                var strategy = new Binacle.Lib.Strategies.DecreasingVolumeSizeFirstFittingOrientation()
                .WithBins(bins)
                .AndItems(null)
                .Build();
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                var strategy = new Binacle.Lib.Strategies.DecreasingVolumeSizeFirstFittingOrientation()
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

            var binsWith0Dimension = testItemsWith0Dimension.Select(x => new Bin(x.ID, x)).ToList();
            var items = testItems.Select(x => new Item(x.ID, x)).ToList();

            Assert.Throws<DimensionException>(() =>
            {
                var strategy = new Binacle.Lib.Strategies.DecreasingVolumeSizeFirstFittingOrientation()
                .WithBins(binsWith0Dimension)
                .AndItems(items)
                .Build();
            });

            var bins = testItems.Select(x => new Bin(x.ID, x)).ToList();
            var itemsWith0Dimension = testItemsWith0Dimension.Select(x => new Item(x.ID, x)).ToList();

            Assert.Throws<DimensionException>(() =>
            {
                var strategy = new Binacle.Lib.Strategies.DecreasingVolumeSizeFirstFittingOrientation()
                .WithBins(bins)
                .AndItems(itemsWith0Dimension)
                .Build();
            });
        }
    }
        
}
