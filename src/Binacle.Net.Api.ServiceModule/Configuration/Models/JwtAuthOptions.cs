namespace Binacle.Net.Api.ServiceModule.Configuration.Models;

internal class JwtAuthOptions
{
	public static string SectionName = "JwtAuth";
	public static string FilePath = "ServiceModule/JwtAuth.json";
	public static string GetEnvironmentFilePath(string environment) => $"ServiceModule/JwtAuth.{environment}.json";

	public string Issuer { get; set; }
	public string Audience { get; set; }
	public string TokenSecret { get; set; }

	public int ExpirationInSeconds { get; set; }
}
