using BenchmarkDotNet.Running;

namespace Binacle.Net.Lib.Benchmarks;

internal class Program
{
    static void Main(string[] args)
    {
        BenchmarkRunner.Run<DecreasingVolumeSize.VersionTests>();
    }
}