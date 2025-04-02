using Binacle.Net.Api.Kernel.Configuration.Models;

namespace Binacle.Net.Api.ServiceModule.Domain.Configuration.Models;

public class UserOptions : IConfigurationOptions
{
	public static string FilePath => "ServiceModule/Users.json";
	public static string SectionName => "Users";
	public static bool Optional => false;
	public static bool ReloadOnChange => true;
	public static string? GetEnvironmentFilePath(string environment) => null;
	
	public string? DefaultAdminUser { get; set; }

	// TODO: add user options
	public ConfiguredUser GetParsedDefaultAdminUser()
	{
		if (string.IsNullOrWhiteSpace(this.DefaultAdminUser))
		{
			throw new InvalidOperationException("DefaultAdminUser is not set");
		}
		var parts = this.DefaultAdminUser.Split(":");
		return new ConfiguredUser
		{
			Email = parts[0],
			Password = parts[1]
		};

	}
}
