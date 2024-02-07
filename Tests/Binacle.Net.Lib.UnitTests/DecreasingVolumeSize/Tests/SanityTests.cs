using Xunit;

namespace Binacle.Net.Lib.UnitTests.DecreasingVolumeSize.Tests;

public class SanityTests : IClassFixture<DecreasingVolumeSizeFixture>
{
    private DecreasingVolumeSizeFixture Fixture { get; }
    
    public SanityTests(DecreasingVolumeSizeFixture fixture)
    {
        Fixture = fixture;
    }

    [Fact]
    public void TestsWork()
    {
        Xunit.Assert.True(true);
    }

    [Fact]
    public void BinCollectionsConfigured()
    {
        Xunit.Assert.NotNull(this.Fixture);
        Xunit.Assert.NotNull(this.Fixture.Bins);

        foreach (var binCollection in this.Fixture.Bins.Values)
        {
            Xunit.Assert.NotEmpty(binCollection);
        }
    }

    [Fact]
    public void ScenariosConfiguredCorrectly()
    {
        Xunit.Assert.NotNull(this.Fixture);
        Xunit.Assert.NotNull(this.Fixture.Scenarios);
        Xunit.Assert.True(this.Fixture.Scenarios.Count > 0);

        foreach(var scenario in this.Fixture.Scenarios.Values)
        {
            var scenarioExpectedSize = scenario.ExpectedSize;
            this.Fixture.Bins.TryGetValue(scenario.BinCollection, out var binCollection);
            Xunit.Assert.NotNull(binCollection);
            if(scenarioExpectedSize != "None")
            {
                var expectedSize = binCollection.FirstOrDefault(x => x.ID == scenarioExpectedSize);
                Xunit.Assert.NotNull(expectedSize);
            }
        }
    }
}
