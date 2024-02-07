using AutoFixture;
using Binacle.Net.Lib.Exceptions;
using Xunit;

namespace Binacle.Net.Lib.UnitTests.DecreasingVolumeSize.Tests;

public class BehavioralTests : IClassFixture<DecreasingVolumeSizeFixture>
{
    private DecreasingVolumeSizeFixture Fixture { get; }
    public Fixture AutoFixture { get; }

    public BehavioralTests(DecreasingVolumeSizeFixture fixture)
    {
        Fixture = fixture;
        AutoFixture = new Fixture();
    }

    [Fact]
    public void WithBinsAndItems_NullOrEmpty_OnBuildThrows_ArgumentNullException()
    {
        var testItems = AutoFixture.CreateMany<Models.TestItem>(2);
        var testBins = AutoFixture.CreateMany<Models.TestBin>(2);


        Assert.Throws<ArgumentNullException>(() =>
        {
            foreach (var strategy in this.Fixture.GetRegisteredStrategies())
            {
                strategy
                .WithBins((IEnumerable<Models.TestBin>)null)
                .AndItems(testItems)
                .Build();
            }

        });

        Assert.Throws<ArgumentNullException>(() =>
        {
            foreach (var strategy in this.Fixture.GetRegisteredStrategies())
            {
                strategy
                .WithBins(new List<Models.TestBin>())
                .AndItems(testItems)
                .Build();
            }

        });

        Assert.Throws<ArgumentNullException>(() =>
        {
            foreach (var strategy in this.Fixture.GetRegisteredStrategies())
            {
                strategy
                .WithBins(testBins)
                .AndItems((IEnumerable<Models.TestItem>)null)
                .Build();
            }
        });

        Assert.Throws<ArgumentNullException>(() =>
        {
            foreach (var strategy in this.Fixture.GetRegisteredStrategies())
            {
                strategy
                .WithBins(testBins)
                .AndItems(new List<Models.TestItem>())
                .Build();
            }
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
            foreach (var strategy in this.Fixture.GetRegisteredStrategies())
            {
                strategy
                .WithBins(testBinsWith0Dimension)
                .AndItems(testItems)
                .Build();
            }
        });

        Assert.Throws<DimensionException>(() =>
        {
            foreach (var strategy in this.Fixture.GetRegisteredStrategies())
            {
                strategy
                .WithBins(testBins)
                .AndItems(testItemsWith0Dimension)
                .Build();
            }
        });
    }
}
