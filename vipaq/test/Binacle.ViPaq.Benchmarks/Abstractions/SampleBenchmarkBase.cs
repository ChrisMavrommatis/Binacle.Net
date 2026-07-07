using BenchmarkDotNet.Attributes;
using Binacle.ViPaq.TestsKernel.Models;
using Binacle.ViPaq.TestsKernel.Samples;

namespace Binacle.ViPaq.Benchmarks.Abstractions;
// [REVIEW-VIPAQ_TEST]
// Shared plumbing for the encode / decode benchmarks: BenchmarkDotNet fans out over the curated sample
// names, and setup resolves the one sample this run measures. The catalog is shared with the size runner,
// so both tools measure the exact same data.
public abstract class SampleBenchmarkBase
{
	[ParamsSource(typeof(BenchmarkCatalog), nameof(BenchmarkCatalog.Names))]
	public string SampleName { get; set; } = "";

	protected PackingSample Sample { get; private set; } = null!;

	[GlobalSetup]
	public virtual void GlobalSetup() => this.Sample = BenchmarkCatalog.GetByName(this.SampleName);
}
