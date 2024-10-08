using Binacle.Net.TestsKernel.Helpers;
using Binacle.Net.TestsKernel.Models;
using Newtonsoft.Json;


namespace Binacle.Net.TestsKernel.Providers;

internal class FileScenarioReader
{
	private class ReadScenario
	{
		public string Name { get; set; }
		public string Bin{ get; set; }
		public bool Fits { get; set; }
		public string[] Items { get; set; }
	}

	public List<Scenario> ReadScenarios(FileInfo file)
	{
		var resultScenarios = new List<Scenario>();
		using (var sr = new StreamReader(file.OpenRead()))
		{
			var readScenarios = JsonConvert.DeserializeObject<List<ReadScenario>>(sr.ReadToEnd());
			if(readScenarios is null)
			{
				return resultScenarios;
			}
			foreach (var readScenario in readScenarios)
			{
				var resultScenario = new Scenario(readScenario.Bin)
				{
					Name = readScenario.Name,
					Fits = readScenario.Fits,
					Items = readScenario.Items.Select(x => 
					{
						var dimensions = DimensionHelper.ParseFromCompactString(x);
						return new TestItem(x, dimensions, dimensions.Quantity);
					}).ToList()
				};
				resultScenarios.Add(resultScenario);
			}
		}

		return resultScenarios;
	}
}


