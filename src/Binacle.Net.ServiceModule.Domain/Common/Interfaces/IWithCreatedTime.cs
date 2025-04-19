namespace Binacle.Net.ServiceModule.Domain.Common.Interfaces;

public interface IWithCreatedTime
{
	DateTimeOffset CreatedAtUtc { get; }
}
