using Binacle.Net.Lib.Tests.Data.Providers;
using Binacle.Net.Lib.Tests.Models;
using Binacle.Net.Lib.UnitTests.Models;
using Newtonsoft.Json;

namespace Binacle.Net.Lib.UnitTests.FirstFitDecreasing;

public sealed class FirstFitDecreasingFixture : IDisposable
{
	private readonly BinTestDataProvider binTestDataProvider;

	public Dictionary<string, List<TestBin>> Bins => this.binTestDataProvider.Bins;

	public readonly Dictionary<string, Scenario> Scenarios;

	public FirstFitDecreasingFixture()
	{
		this.binTestDataProvider = new BinTestDataProvider(Data.Constants.SolutionRootBasePath);

		this.Scenarios = new Dictionary<string, Scenario>();
		var scenariosDirectoryInfo = new System.IO.DirectoryInfo($"{Data.Constants.DataBasePath}/Scenarios");
		foreach (var scenarioCollectionFileInfo in scenariosDirectoryInfo.GetFiles())
		{
			var scenarioCollectionName = Path.GetFileNameWithoutExtension(scenarioCollectionFileInfo.Name);
			using (var sr = new StreamReader(scenarioCollectionFileInfo.OpenRead()))
			{
				var scenarios = JsonConvert.DeserializeObject<List<Scenario>>(sr.ReadToEnd());
				foreach (var scenario in scenarios)
				{
					this.Scenarios.Add($"{scenarioCollectionName}_{scenario.Name}", scenario);
				}
			}
		}
	}

	public void Dispose()
	{

	}

}

