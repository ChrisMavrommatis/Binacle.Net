namespace Binacle.Lib.PerformanceTests.Models;

internal class TestResultList : List<TestResult>
{
	public required string Title { get; set; }
	public string? Description { get; set; }
	public string? Filename { get; set; }
}
