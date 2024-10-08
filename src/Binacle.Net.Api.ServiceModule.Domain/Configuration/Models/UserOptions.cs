namespace Binacle.Net.Api.ServiceModule.Domain.Configuration.Models;

public class UserOptions
{
	public static string SectionName = "Users";
	public static string FilePath = "ServiceModule/Users.json";
	public string DefaultAdminUser { get; set; }

	// TODO: add user options
	public ConfiguredUser GetParsedDefaultAdminUser()
	{
		var parts = this.DefaultAdminUser.Split(":");
		return new ConfiguredUser
		{
			Email = parts[0],
			Password = parts[1]
		};

	}
}
