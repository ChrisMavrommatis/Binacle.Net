namespace Binacle.Net.Api.ServiceModule.Domain.Models;

public interface IWithCreatedTime
{
	public DateTimeOffset CreatedAtUtc { get; set; }
}
