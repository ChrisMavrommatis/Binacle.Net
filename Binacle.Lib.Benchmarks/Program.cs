using BenchmarkDotNet.Running;

namespace Binacle.Lib.Benchmarks
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<Strategies.DecreasingVolumeSizeFirstFittingOrientationBenchmarks>();
        }
    }
}