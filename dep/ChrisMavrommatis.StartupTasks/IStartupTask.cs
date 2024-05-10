namespace ChrisMavrommatis.StartupTasks;

public interface IStartupTask
{
	Task ExecuteAsync(CancellationToken cancellationToken = default);
}
