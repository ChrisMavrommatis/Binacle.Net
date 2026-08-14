
namespace Binacle.Lib.TestsKernel.ResultSelection.Models;

public record CollectionScenario(
    string CollectionKey,
    Scenario Scenario
);

public class Scenario
{
    public required string Name { get; init; }
    public required string ExpectedResult { get; init; }
    public required Dictionary<string, OperationResult> Results { get; init; }
    
    public override string ToString() => Name;

    public static Scenario Create(
        string name, 
        string expectedResult,
        Dictionary<string, string> results
        )
    {
        var parsedResults = Helpers.OperationResultHelper.ParseManyFromCompactStrings(results);
        return new Scenario()
        {
            Name = name,
            ExpectedResult = expectedResult,
            Results = parsedResults
        };
    }
}

