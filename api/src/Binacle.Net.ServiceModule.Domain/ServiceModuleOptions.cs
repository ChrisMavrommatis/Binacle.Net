namespace Binacle.Net.ServiceModule.Domain;

public class ServiceModuleOptions
{
	public string DefaultAdminAccount { get; set; } = "admin@binacle.net:B1n4cl3Adm!n";
	
	public static ConfiguredAccountCredentials ParseAccountCredentials(string accountCredentials)
	{
		if (string.IsNullOrWhiteSpace(accountCredentials))
		{
			throw new ArgumentNullException(nameof(accountCredentials), "Account credentials cannot be null or empty");
		}

		var parts = accountCredentials.Split(":");
		return new ConfiguredAccountCredentials
		{
			Username = parts[0],
			Email = parts[0],
			Password = parts[1]
		};
	}
}


public class ConfiguredAccountCredentials
{
	public required string Username { get; init; }
	public required string Email { get; init; }
	public required string Password { get; init; }
}
