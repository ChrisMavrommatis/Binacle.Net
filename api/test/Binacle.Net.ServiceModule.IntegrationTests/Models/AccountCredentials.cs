namespace Binacle.Net.ServiceModule.IntegrationTests.Models;

public record AccountCredentials(
	Guid Id,
	string Username,
	string Email,
	string Password
);
