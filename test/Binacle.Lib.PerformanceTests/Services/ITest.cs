using Binacle.Lib.PerformanceTests.Models;

namespace Binacle.Lib.PerformanceTests.Services;

internal interface ITest
{
	TestResultList Run();
}
