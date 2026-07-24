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
		// The whole section is optional, and no origins means no browser is allowed through — a valid, closed
		// default. What is worth refusing is an origin the browser can never match, because that fails at request
		// time in someone else's console, not here.
		When(x => x.CoreApi?.AllowedOrigins is not null, () =>
		{
			RuleForEach(x => x.CoreApi!.AllowedOrigins)
				.Must(BeAMatchableOrigin)
				.WithMessage(
					"'{PropertyValue}' is not a usable origin. Use scheme, host and optional port with nothing "
					+ "after it, such as https://example.com or http://localhost:5173. No trailing slash, path "
					+ "or query. Use '*' to allow any origin."
				);
		});
	}

	// The browser compares an origin as an exact string, so a trailing slash or a path never matches anything and
	// the operator sees only a blocked request. Refusing at startup is the only place this is visible.
	private static bool BeAMatchableOrigin(string? origin)
	{
		if (string.IsNullOrWhiteSpace(origin))
		{
			return false;
		}

		if (origin == "*")
		{
			return true;
		}

		if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
		{
			return false;
		}

		return (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
			&& uri.PathAndQuery == "/"
			&& !origin.EndsWith('/')
			&& string.IsNullOrEmpty(uri.Fragment);
	}
}
