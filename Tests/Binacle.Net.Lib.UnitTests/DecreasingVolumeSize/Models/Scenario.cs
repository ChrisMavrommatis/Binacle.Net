namespace Binacle.Net.Lib.UnitTests.DecreasingVolumeSize.Models
{
    public sealed class Scenario
    {
        public string Name { get; set; }
        public string BinCollection { get; set; }
        public string ExpectedSize { get; set; }
        public List<Models.TestItem> Items { get; set; }
    }
}
