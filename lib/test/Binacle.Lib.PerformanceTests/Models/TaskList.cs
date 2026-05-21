namespace Binacle.Lib.PerformanceTests.Models;

internal class TaskList<T> : List<Task<T>>
{
	public TaskList()
	{
	}

	public TaskList(IEnumerable<Task<T>> collection)
		: base(collection)
	{
	}
}
