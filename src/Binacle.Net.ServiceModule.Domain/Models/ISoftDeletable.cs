namespace Binacle.Net.ServiceModule.Domain.Models;

public interface ISoftDeletable
{
	public bool IsDeleted { get; set; }
	public DateTimeOffset? DeletedAtUtc { get; set; }
}
