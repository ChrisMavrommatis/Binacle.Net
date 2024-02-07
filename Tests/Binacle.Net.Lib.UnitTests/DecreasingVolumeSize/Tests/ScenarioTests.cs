using AutoFixture;
using Binacle.Net.Lib.Abstractions.Strategies;
using Xunit;

namespace Binacle.Net.Lib.UnitTests.DecreasingVolumeSize.Tests;

public class ScenarioTests : IClassFixture<DecreasingVolumeSizeFixture>
{
    private DecreasingVolumeSizeFixture Fixture { get; }
    public Fixture AutoFixture { get; }

    public ScenarioTests(DecreasingVolumeSizeFixture fixture)
    {
        Fixture = fixture;
    }

    [Theory]
    [ClassData(typeof(Data.BaselineTestDataProvider))]
    public void Baseline_v1(Models.Scenario scenario) 
        => this.RunScenarioTest(new Binacle.Net.Lib.Strategies.DecreasingVolumeSize_v1(), scenario);

    [Theory]
    [ClassData(typeof(Data.SimpleTestDataProvider))]
    public void Simple_v1(Models.Scenario scenario) 
        => this.RunScenarioTest(new Binacle.Net.Lib.Strategies.DecreasingVolumeSize_v1(), scenario);

    [Theory]
    [ClassData(typeof(Data.ComplexTestDataProvider))]
    public void Complex_v1(Models.Scenario scenario) 
        => this.RunScenarioTest(new Binacle.Net.Lib.Strategies.DecreasingVolumeSize_v1(), scenario);

    [Theory]
    [ClassData(typeof(Data.BaselineTestDataProvider))]
    public void Baseline_v2(Models.Scenario scenario)
        => this.RunScenarioTest(new Binacle.Net.Lib.Strategies.DecreasingVolumeSize_v2(), scenario);

    [Theory]
    [ClassData(typeof(Data.SimpleTestDataProvider))]
    public void Simple_v2(Models.Scenario scenario)
        => this.RunScenarioTest(new Binacle.Net.Lib.Strategies.DecreasingVolumeSize_v2(), scenario);

    [Theory]
    [ClassData(typeof(Data.ComplexTestDataProvider))]
    public void Complex_v2(Models.Scenario scenario)
        => this.RunScenarioTest(new Binacle.Net.Lib.Strategies.DecreasingVolumeSize_v2(), scenario);

    private void RunScenarioTest<TStrategy>(TStrategy strategy, Models.Scenario scenario)
        where TStrategy : class, IBinFittingStrategy
    {
        var binCollection = this.Fixture.Bins[scenario.BinCollection];

        var operation = strategy
         .WithBins(binCollection)
         .AndItems(scenario.Items)
         .Build();

        var result = operation.Execute();

        if (scenario.ExpectedSize != "None")
        {
            Xunit.Assert.Equal(scenario.ExpectedSize, result.FoundBin.ID);
        }
        else
        {
            Xunit.Assert.Null(result.FoundBin);
        }
    }

}

