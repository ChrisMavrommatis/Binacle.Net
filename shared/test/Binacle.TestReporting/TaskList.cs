namespace Binacle.TestReporting;

// A small alias so the runner can collect result tasks without spelling out List<Task<T>> each time.
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
