namespace Binacle.Net.ServiceModule.IntegrationTests.Models;

public class AccountCredentials
{
	public Guid? Id { get; set; }
	public required string Username { get; init; }
	public required string Email { get; init; }
	public required string Password { get; init; }
}
