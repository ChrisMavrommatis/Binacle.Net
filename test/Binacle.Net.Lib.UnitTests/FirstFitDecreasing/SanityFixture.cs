using Binacle.Net.Lib.UnitTests.Data.Models;
using Binacle.Net.TestsKernel.Data.Providers;
using Binacle.Net.TestsKernel.Helpers;
using Binacle.Net.TestsKernel.Models;
using Newtonsoft.Json;

namespace Binacle.Net.Lib.UnitTests.FirstFitDecreasing;

public sealed class SanityFixture : IDisposable
{
	private readonly BinTestDataProvider binTestDataProvider;

	public Dictionary<string, List<TestBin>> Bins => this.binTestDataProvider.Bins;

	public readonly Dictionary<string, Scenario> CompactScenarios;
	public readonly Dictionary<string, Scenario> NormalScenarios;

	public SanityFixture()
	{
		this.binTestDataProvider = new BinTestDataProvider(Data.Constants.SolutionRootBasePath);
		this.CompactScenarios = this.ReadCompactScenarios();
		this.NormalScenarios = this.ReadNormalScenarios();
	}

	private Dictionary<string, Scenario> ReadCompactScenarios()
	{
		var scenarios = new Dictionary<string, Scenario>();
		var scenariosDirectoryInfo = new System.IO.DirectoryInfo($"{Data.Constants.DataBasePath}/Scenarios/Compact");
		foreach (var scenarioCollectionFileInfo in scenariosDirectoryInfo.GetFiles())
		{
			var scenarioCollectionName = Path.GetFileNameWithoutExtension(scenarioCollectionFileInfo.Name);
			using (var sr = new StreamReader(scenarioCollectionFileInfo.OpenRead()))
			{
				var compactScenarios = JsonConvert.DeserializeObject<List<CompactScenario>>(sr.ReadToEnd());
				foreach (var compactScenario in compactScenarios)
				{
					var scenario = new Scenario
					{
						Name = compactScenario.Name,
						ExpectedSize = compactScenario.ExpectedSize,
						BinCollection = compactScenario.BinCollection,
						Items = compactScenario.Items.Select(x => DimensionHelper.ParseFromCompactString(x)).ToList()
					};

					scenarios.Add($"{scenarioCollectionName}_{scenario.Name}", scenario);
				}
			}
		}

		return scenarios;
	}

	private Dictionary<string, Scenario> ReadNormalScenarios()
	{
		var scenarios = new Dictionary<string, Scenario>();

		var scenariosDirectoryInfo = new System.IO.DirectoryInfo($"{Data.Constants.DataBasePath}/Scenarios/Normal");
		foreach (var scenarioCollectionFileInfo in scenariosDirectoryInfo.GetFiles())
		{
			var scenarioCollectionName = Path.GetFileNameWithoutExtension(scenarioCollectionFileInfo.Name);
			using (var sr = new StreamReader(scenarioCollectionFileInfo.OpenRead()))
			{
				var normalScenarios = JsonConvert.DeserializeObject<List<Scenario>>(sr.ReadToEnd());
				foreach (var normalScenario in normalScenarios)
				{
					scenarios.Add($"{scenarioCollectionName}_{normalScenario.Name}", normalScenario);
				}
			}
		}
		return scenarios;

	}

	public void Dispose()
	{

	}
}
