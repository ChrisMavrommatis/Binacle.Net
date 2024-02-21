using Binacle.Net.Lib.Tests.Models;

namespace Binacle.Net.Lib.UnitTests.Models;

public sealed class Scenario
{
    public string Name { get; set; }
    public string BinCollection { get; set; }
    public string ExpectedSize { get; set; }
    public List<TestItem> Items { get; set; }
}
