namespace Binacle.Lib.Benchmarks.Order;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
internal class BenchmarkOrderAttribute : Attribute
{
	public int Order { get; }

	public BenchmarkOrderAttribute(int order)
	{
		Order = order;
	}
}
