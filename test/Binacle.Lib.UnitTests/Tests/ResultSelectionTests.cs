using Binacle.Lib.Abstractions;
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Models;

namespace Binacle.Lib.UnitTests;

[Trait("Scenario Tests", "Actual calculation for the algorithms.")]
public class ResultSelectionTests : IClassFixture<ResultSelectionTestingFixture>
{
	private readonly ResultSelectionTestingFixture fixture;

	public ResultSelectionTests(ResultSelectionTestingFixture fixture)
	{
		this.fixture = fixture;
	}

	[Fact]
    public void Best_Algorithm_OneFullyPacked()
    {
	    Dictionary<string, OperationResult> results = new()
	    {
		    {"FFD_v2", this.fixture.MakeResult("60x40x10", "FFD_v2", OperationResultStatus.PartiallyPacked, 72.13m, 96.11m)},
		    {"BFD_v2", this.fixture.MakeResult("60x40x10", "BFD_v2", OperationResultStatus.FullyPacked, 75.05m, 100m)},
		    {"WFD_v2", this.fixture.MakeResult("60x40x10", "WFD_v2", OperationResultStatus.PartiallyPacked, 69.22m, 92.23m)}
	    };

	    IResultSelectionStrategy[] algorithmStrategies =
	    [
		    new Lib.ResultSelection.BestAlgorithm_v1(),
		    new Lib.ResultSelection.BestAlgorithm_v2(),
	    ];
	    
	    foreach(var strategy in algorithmStrategies)
	    {
		    var selected = strategy.Select(results);
		    var algorithmIdentifier = selected.AlgorithmInfo.GetAlgorithmIdentifierName();
		    algorithmIdentifier.ShouldBe("BFD_v2");
	    }
    }
    
	[Fact]
	public void BestAlgorithm_MultipleFullyPacked()
	{
		Dictionary<string, OperationResult> results = new()
		{
			{"FFD_v2", this.fixture.MakeResult("60x40x10", "FFD_v2", OperationResultStatus.FullyPacked, 66.55m, 100m)},
			{"BFD_v2", this.fixture.MakeResult("60x40x10", "BFD_v2", OperationResultStatus.FullyPacked, 66.55m, 100m)},
			{"WFD_v2", this.fixture.MakeResult("60x40x10", "WFD_v2", OperationResultStatus.FullyPacked, 66.55m, 100m)}
		};

		IResultSelectionStrategy[] algorithmStrategies =
		[
			new Lib.ResultSelection.BestAlgorithm_v1(),
			new Lib.ResultSelection.BestAlgorithm_v2(),
		];
	    
		foreach(var strategy in algorithmStrategies)
		{
			var selected = strategy.Select(results);
			var algorithmIdentifier = selected.AlgorithmInfo.GetAlgorithmIdentifierName();
			algorithmIdentifier.ShouldBe("FFD_v2");
		}
	}
	
	[Fact]
	public void BestAlgorithm_AllPartial()
	{
		Dictionary<string, OperationResult> results = new()
		{
			{"FFD_v2", this.fixture.MakeResult("60x40x10", "FFD_v2", OperationResultStatus.PartiallyPacked, 78.08m, 78.23m)},
			{"BFD_v2", this.fixture.MakeResult("60x40x10", "BFD_v2", OperationResultStatus.PartiallyPacked, 80.30m, 80.46m)},
			{"WFD_v2", this.fixture.MakeResult("60x40x10", "WFD_v2", OperationResultStatus.PartiallyPacked, 78.08m, 78.23m)}
		};

		IResultSelectionStrategy[] algorithmStrategies =
		[
			new Lib.ResultSelection.BestAlgorithm_v1(),
			new Lib.ResultSelection.BestAlgorithm_v2(),
		];
	    
		foreach(var strategy in algorithmStrategies)
		{
			var selected = strategy.Select(results);
			var algorithmIdentifier = selected.AlgorithmInfo.GetAlgorithmIdentifierName();
			algorithmIdentifier.ShouldBe("BFD_v2");
		}
	}
}
