using System.Numerics;

namespace Binacle.Lib.PerformanceTests.Models;

internal class MeasurementResult<T>
	where T : struct,  INumber<T>, IComparable<T>
{
	public required string Algorithm { get; init; }
	public required T Min { get; init; }
	public required double Mean { get; init; }
	public required double Median { get; init; }
	public required T Max { get; init; }
}
                                                                                                                
