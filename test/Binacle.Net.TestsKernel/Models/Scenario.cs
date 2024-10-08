namespace Binacle.Net.TestsKernel.Models;

public sealed class Scenario : BinScenarioBase
{
	public Scenario(string binString) : base(binString)
	{
	}
	public bool Fits { get; set; }
	public string Name { get; set; }
	public List<TestItem> Items { get; set; }
}

