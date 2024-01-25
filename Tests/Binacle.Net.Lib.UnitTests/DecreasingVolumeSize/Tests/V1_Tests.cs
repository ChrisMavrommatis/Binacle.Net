using AutoFixture;
using Binacle.Net.Lib.Exceptions;
using Xunit;

namespace Binacle.Net.Lib.UnitTests.DecreasingVolumeSize.Tests
{
    public class V1_Tests : IClassFixture<DecreasingVolumeSizeFixture>
    {
        private DecreasingVolumeSizeFixture Fixture { get; }
        public Fixture AutoFixture { get; }

        public V1_Tests(DecreasingVolumeSizeFixture fixture)
        {
            Fixture = fixture;
            AutoFixture = new Fixture();
            //this.AutoFixture.Customize<IWithReadOnlyDimensions<int>>(x => x.FromFactory(() => new TestItem()));
        }

        [Fact]
        public void WithBinsAndItems_NullOrEmpty_OnBuildThrows_ArgumentNullException()
        {
            var testItems = AutoFixture.CreateMany<Models.TestItem>(2);
            var testBins = AutoFixture.CreateMany<Models.TestBin>(2);

            Assert.Throws<ArgumentNullException>(() =>
            {
                var strategy = new Strategies.DecreasingVolumeSize_v1()
                .WithBins((IEnumerable<Models.TestBin>)null)
                .AndItems(testItems)
                .Build();
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                var strategy = new Strategies.DecreasingVolumeSize_v1()
                .WithBins(new List<Models.TestBin>())
                .AndItems(testItems)
                .Build();
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                var strategy = new Strategies.DecreasingVolumeSize_v1()
                .WithBins(testBins)
                .AndItems((IEnumerable<Models.TestItem>)null)
                .Build();
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                var strategy = new Strategies.DecreasingVolumeSize_v1()
                .WithBins(testBins)
                .AndItems(new List<Models.TestItem>())
                .Build();
            });
        }

        [Fact]
        public void With0DimensionBinsAndItems_OnBuildThrows_DimensionException()
        {
            var testItems = AutoFixture.CreateMany<Models.TestItem>(2);
            var testBins = AutoFixture.CreateMany<Models.TestBin>(2);
            var testItemsWith0Dimension = AutoFixture.Build<Models.TestItem>()
                .With(x => x.Width, 0)
                .CreateMany(2);

            var testBinsWith0Dimension = AutoFixture.Build<Models.TestBin>()
               .With(x => x.Width, 0)
               .CreateMany(2);

            Assert.Throws<DimensionException>(() =>
            {
                var strategy = new Strategies.DecreasingVolumeSize_v1()
                .WithBins(testBinsWith0Dimension)
                .AndItems(testItems)
                .Build();
            });

            Assert.Throws<DimensionException>(() =>
            {
                var strategy = new Strategies.DecreasingVolumeSize_v1()
                .WithBins(testBins)
                .AndItems(testItemsWith0Dimension)
                .Build();
            });
        }

        //[Theory]
        
        //public void BaselineScenarios(Dictionary<string, Scenario> scenarios)
        //{
        //    var baseLineScenarios = this.Fixture.Scenarios["Baseline"];

        //    foreach(var scenario in baseLineScenarios)
        //    {

        //    }
        //}
    }

}
