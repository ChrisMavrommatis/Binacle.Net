namespace Binacle.Net.Api.ServiceModule.Domain.Users.Entities;

public class User
{
	public string Email { get; set; }
	public string Group { get; set; }
	public string HashedPassword { get; set; }
	public string Salt { get; set; }
	public bool IsActive { get; set; }
}
