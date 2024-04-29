namespace Binacle.Net.Api.Tests.TestPriority;


[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
internal class TestPriorityAttribute : Attribute
{
	public TestPriorityAttribute(int priority)
	{
		Priority = priority;
	}

	public int Priority { get; private set; }
}
