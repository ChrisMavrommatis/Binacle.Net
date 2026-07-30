namespace Binacle.ViPaq.PerformanceTests.PreReportChecks;

// A fail-fast gate run before the reports: throws on failure, writes no report file (unlike `ITest`). Registered
// via `AddPreReportChecks`, run by `RunPreReportChecks` before the report `TestRunner`.
internal interface IPreReportCheck
{
	void Run();
}
