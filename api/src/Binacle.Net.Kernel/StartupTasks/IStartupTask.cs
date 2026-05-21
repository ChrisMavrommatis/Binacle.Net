namespace Binacle.Net.Kernel.StartupTasks;

public interface IStartupTask
{
    Task ExecuteAsync(CancellationToken cancellationToken = default);
}
