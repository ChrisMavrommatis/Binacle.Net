using Binacle.Net.Kernel.Configuration.Models;
using FluentValidation;

namespace Binacle.Net.Configuration;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

internal static class CorsPolicy
{
	public const string CoreApi = "CoreApi";

}

public class CorsOptions: IConfigurationOptions
{
	public static string FilePath => "Cors.json";
	public static string SectionName => "Cors";
	public static bool Optional => true;
	public static bool ReloadOnChange => true;
	public static string? GetEnvironmentFilePath(string environment) => $"Cors.{environment}.json";

	
	public CorsPolicyOptions? CoreApi {get;set;}
}

public class CorsPolicyOptions
{
	public string[]? AllowedOrigins {get;set;}
}


internal class CorsOptionsOptionsValidator : AbstractValidator<CorsOptions>
{
	public CorsOptionsOptionsValidator()
	{
		
	}
}
