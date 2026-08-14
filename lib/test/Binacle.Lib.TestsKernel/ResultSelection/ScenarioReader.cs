using System.Text.Json;
using Binacle.Lib.TestsKernel.Files;

namespace Binacle.Lib.TestsKernel.ResultSelection;

internal static class ScenarioReader
{ 
    private class ReadScenario
    {
        public string? Name { get; set; }
        public string? ExpectedResult{ get; set; }
        public Dictionary<string, string>? Results { get; set; }
    }

    public static List<Models.Scenario> ReadScenarios(IFile file)
    {
        var resultScenarios = new List<Models.Scenario>();
        using (var sr = new StreamReader(file.OpenRead()))
        {
            var readScenarios = JsonSerializer.Deserialize<List<ReadScenario>>(sr.ReadToEnd());
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
                if(string.IsNullOrWhiteSpace(readScenario.ExpectedResult))
                {
                    throw new ArgumentNullException("No expected result found in scenario");
                }

                if (readScenario.Results is null || readScenario.Results.Count < 1)
                {
                    throw new ArgumentNullException("No results found in scenario");
                }
             
                if (!readScenario.Results.ContainsKey(readScenario.ExpectedResult))
                {
                    throw new ArgumentException($"Expected result '{readScenario.ExpectedResult}' is not found in results");
                }

                var resultScenario = Models.Scenario.Create(
                    readScenario.Name,
                    readScenario.ExpectedResult,
                    readScenario.Results
                );

                resultScenarios.Add(resultScenario);
            }
        }

        return resultScenarios;
    }
}
