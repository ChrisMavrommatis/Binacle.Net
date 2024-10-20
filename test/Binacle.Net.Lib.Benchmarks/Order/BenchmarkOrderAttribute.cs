namespace Binacle.Net.Lib.Benchmarks.Order;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class BenchmarkOrderAttribute : Attribute
{
	public int Order { get; }

	public BenchmarkOrderAttribute(int order)
	{
		Order = order;
	}
}
