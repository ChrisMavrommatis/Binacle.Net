namespace Binacle.Net.ServiceModule.Domain.Common.Interfaces;

public interface ISoftDeletable
{
	bool IsDeleted { get; }
	DateTimeOffset? DeletedAtUtc { get; }
}
