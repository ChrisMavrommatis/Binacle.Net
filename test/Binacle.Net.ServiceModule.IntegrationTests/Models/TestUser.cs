namespace Binacle.Net.ServiceModule.IntegrationTests.Models;

public class TestAccount
{
	public Guid Id { get; set; }
	public required string Username { get; set; }
	public required string Password { get; set; }
}
