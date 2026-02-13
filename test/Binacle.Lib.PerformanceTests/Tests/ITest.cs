using Binacle.Lib.PerformanceTests.Models;

namespace Binacle.Lib.PerformanceTests.Tests;

internal interface ITest
{
	Models.ResultFile File { get; }
	TestResult Run();
}
