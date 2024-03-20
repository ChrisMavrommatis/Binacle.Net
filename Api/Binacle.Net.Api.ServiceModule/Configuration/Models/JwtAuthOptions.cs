namespace Binacle.Net.Api.ServiceModule.Configuration.Models;

internal class JwtAuthOptions
{
	public static string SectionName = "JwtAuth";
	public static string FilePath = "JwtAuth.json";
	public static string EvironmentPrefix = "JWT__";
	public static string GetEnvironmentFilePath(string environment) => $"JwtAuth.{environment}.json";

	public string Issuer { get; set; }
	public string Audience { get; set; }
	public string TokenSecret { get; set; }

	public int ExpirationInSeconds { get; set; }

	public bool IsConfigured() 
	{
		return !string.IsNullOrWhiteSpace(this.Issuer)
			&& !string.IsNullOrWhiteSpace(this.Audience)
			&& !string.IsNullOrWhiteSpace(this.TokenSecret);
	}
}
