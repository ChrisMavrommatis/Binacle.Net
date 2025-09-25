namespace Binacle.Net.ServiceModule.IntegrationTests.Models;

public record AccountCredentialsWithSubscription(
	Guid Id,
	string Username,
	string Email,
	string Password,
	Guid SubscriptionId
);
