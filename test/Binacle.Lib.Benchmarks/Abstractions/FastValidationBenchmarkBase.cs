using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Benchmarks.Order;
using Binacle.TestsKernel;
using Binacle.TestsKernel.Models;
using Binacle.TestsKernel.ScenarioProviders;

namespace Binacle.Lib.Benchmarks.Abstractions;

public abstract class FastValidationBenchmarkBase
{

	protected abstract Scenario? GetScenario();
	
	protected abstract AlgorithmOperation AlgorithmOperation { get; }
	protected Scenario? Scenario { get; private set; }
	
	
	[GlobalSetup]
	public void GlobalSetup()
	{
		 this.Scenario = this.GetScenario();
	}
	
	[GlobalCleanup]
	public void GlobalCleanup()
	{
	}
	
	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public OperationResult FFD_v1()
		=> this.Run(AlgorithmFactories.FFD_v1);

	[Benchmark]
	[BenchmarkOrder(20)]
	public OperationResult FFD_v2()
		=> this.Run(AlgorithmFactories.FFD_v2);
	
	[Benchmark]
	[BenchmarkOrder(30)]
	public OperationResult WFD_v1()
		=> this.Run(AlgorithmFactories.WFD_v1);

	[Benchmark]
	[BenchmarkOrder(40)]
	public OperationResult WFD_v2()
		=> this.Run(AlgorithmFactories.WFD_v2);
	
	[Benchmark]
	[BenchmarkOrder(50)]
	public OperationResult BFD_v1()
		=> this.Run(AlgorithmFactories.BFD_v1);

	[Benchmark]
	[BenchmarkOrder(60)]
	public OperationResult BFD_v2()
		=> this.Run(AlgorithmFactories.BFD_v2);
	
	protected OperationResult Run(TestAlgorithmFactory<IPackingAlgorithm> algorithmFactory)
	{
		var algorithmInstance = algorithmFactory(this.Scenario!.Bin, this.Scenario!.Items);
		var result = algorithmInstance.Execute(new TestOperationParameters()
		{
			Operation = this.AlgorithmOperation
		});
		return result;
	}
}
