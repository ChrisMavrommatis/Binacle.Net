namespace Binacle.Net.Lib.UnitTests.DecreasingVolumeSize.Data;

internal class BaselineTestDataProvider : ScenarioFileTestDataProvider
{
    public BaselineTestDataProvider() : base($"{DecreasingVolumeSizeFixture.BaseBath}\\Scenarios\\Baseline.json")
    {
       
    }
}
