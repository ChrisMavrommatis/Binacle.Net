using Binacle.Net.TestsKernel.Helpers;
using Binacle.Net.TestsKernel.Models;
using Newtonsoft.Json;


namespace Binacle.Net.TestsKernel.Providers;

internal class EmbeddedResourceFileScenarioReader
{ 
	private class ReadScenario
	{
		public string? Name { get; set; }
		public string? Bin{ get; set; }
		public string? Result { get; set; }
		public string[]? Items { get; set; }
	}

	public List<Scenario> ReadScenarios(EmbeddedResourceFile file)
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
				if (string.IsNullOrWhiteSpace(readScenario.Name))
				{
					throw new ArgumentNullException("No name found in scenario");
				}
				if (string.IsNullOrWhiteSpace(readScenario.Bin))
				{
					throw new ArgumentNullException("No bin found in scenario");
				}
				if (string.IsNullOrWhiteSpace(readScenario.Result))
				{
					throw new ArgumentNullException("No result found in scenario");
				}
				if (readScenario.Items is null || readScenario.Items.Length < 1)
				{
					throw new ArgumentNullException("No items found in scenario");
				}


				var items = readScenario.Items.Select(x =>
				{
					var dimensions = DimensionHelper.ParseFromCompactString(x);
					return new TestItem(x, dimensions, dimensions.Quantity);
				}).ToList();

				var resultScenario = Scenario.Create(
					readScenario.Name,
					readScenario.Bin,
					items,
					readScenario.Result
				);

				resultScenarios.Add(resultScenario);
			}
		}

		return resultScenarios;
	}
}


