using Binacle.Net.ServiceModule.Domain.Models;

namespace Binacle.Net.ServiceModule.Domain.Users.Entities;

public class User: IWithEmail, IActivatable, IWithCreatedTime, ISoftDeletable
{
	public required string Email { get; set; }
	public required string NormalizedEmail { get; set; }
	public required string Group { get; set; }
	public required string HashedPassword { get; set; }
	public required string Salt { get; set; }
	public required DateTimeOffset CreatedAtUtc { get; set; }
	public required bool IsActive { get; set; }
	public required bool IsDeleted { get; set; }
	public DateTimeOffset? DeletedAtUtc { get; set; }
}
