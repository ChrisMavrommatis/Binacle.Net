namespace Binacle.Net.ServiceModule.Domain.Models;

public interface IWithCreatedTime
{
	public DateTimeOffset CreatedAtUtc { get; set; }
}
