namespace Binacle.Net.Lib.UnitTests.DecreasingVolumeSize.Data;

internal class SimpleTestDataProvider : ScenarioFileTestDataProvider
{
    public SimpleTestDataProvider() : base($"{DecreasingVolumeSizeFixture.BaseBath}/Scenarios/Simple.json")
    {

    }
}
