using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions;
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.AlgorithmProcessing;
using Binacle.Lib.Benchmarks.Order;
using Binacle.Lib.Benchmarks.Providers;
using Binacle.TestsKernel.Models;
using Binacle.TestsKernel.Algorithms.Models;
using Binacle.TestsKernel.Algorithms.Providers;

namespace Binacle.Lib.Benchmarks.Abstractions;

public abstract class AlgorithmRacingBenchmarksBase
{
	private ParallelAlgorithmProcessor parallelAlgorithmProcessor = null!;
	private LoopAlgorithmProcessor loopAlgorithmProcessor = null!;

	[ParamsSource(typeof(BischoffCuratedProblemsProvider), nameof(BischoffCuratedProblemsProvider.GetBenchmarkScenarios))]
	public string? Description { get; set; }
	
	[ParamsSource(typeof(ConcurrencyProvider), nameof(ConcurrencyProvider.GetProcessorCount))]
	public int ProcessorCount { get; set; }

	[Params("BFD,WFD", "FFD,BFD", "FFD,WFD", "FFD,BFD,WFD")]
	public string Algorithms { get; set; } = null!;
	
	public Scenario? Scenario { get; set; }
	
	[GlobalSetup]
	public void GlobalSetup()
	{
		var algorithms = this.Algorithms.Split(',').Select(Enum.Parse<Algorithm>).ToArray();
		this.loopAlgorithmProcessor = new LoopAlgorithmProcessor(algorithms, this.AlgorithmFactory);
		this.parallelAlgorithmProcessor = new ParallelAlgorithmProcessor(algorithms, this.AlgorithmFactory, this.ProcessorCount);
		var scenarioName = BischoffCuratedProblemsProvider.ScenarioDescriptions[this.Description!];
		this.Scenario = BischoffSuiteScenarioProvider.GetScenarioByName(scenarioName);
	}
	
	[GlobalCleanup]
	public void GlobalCleanup()
	{
	}
	
	protected abstract AlgorithmOperation AlgorithmOperation { get; }
	protected abstract IAlgorithmFactory AlgorithmFactory { get; }
	
	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public IDictionary<string,OperationResult> Loop()
	{
		return this.loopAlgorithmProcessor.Process(
			this.Scenario!.Bin,
			this.Scenario.Items,
			new TestOperationParameters()
			{
				Operation = this.AlgorithmOperation
			}
		);
	}

	[Benchmark]
	[BenchmarkOrder(20)]
	public IDictionary<string, OperationResult> Parallel()
	{
		return this.parallelAlgorithmProcessor.Process(
			this.Scenario!.Bin,
			this.Scenario.Items,
			new TestOperationParameters()
			{
				Operation = this.AlgorithmOperation
			}
		);
	}
}
