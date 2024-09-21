namespace Binacle.Net.Api.ServiceModule.Domain.Configuration.Models;

public class DefaultsOptions
{
	public static string SectionName = "Defaults";
	public static string FilePath = "ServiceModule/Defaults.json";
	public string AdminUser { get; set; }

	public ConfiguredUser GetParsedAdminUser()
	{
		var parts = this.AdminUser.Split(":");
		return new ConfiguredUser
		{
			Email = parts[0],
			Password = parts[1]
		};

	}
}
